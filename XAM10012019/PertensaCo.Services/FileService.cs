using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PertensaCo.Common.Extensions;
using PertensaCo.Services.Contracts;
using static PertensaCo.Common.Constants.HTTPConstants;

namespace PertensaCo.Services
{
	public class FileService : IFileService
	{
		private readonly IHostingEnvironment environment;
		private readonly ILogger<FileService> logger;

		public FileService(IHostingEnvironment environment, ILogger<FileService> logger)
		{
			this.environment = environment;
			this.logger = logger;
		}

		/// <summary>https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms775107(v=vs.85)</summary>
		/// <param name="pBC">A pointer to the IBindCtx interface.</param>
		/// <param name="pwzUrl">A pointer to a string value that contains the URL of the data. Can be set to NULL if pBuffer contains the data to be sniffed.</param>
		/// <param name="pBuffer">A pointer to the buffer that contains the data to be sniffed. Can be set to NULL if pwzUrl contains a valid URL.</param>
		/// <param name="cbSize">An unsigned long integer value that contains the size of the buffer.</param>
		/// <param name="pwzMimeProposed">A pointer to a string value that contains the proposed MIME type. This value is authoritative if type cannot be determined from the data. If the proposed type contains a semi-colon (;) it is removed. This parameter can be set to NULL.</param>
		/// <param name="dwMimeFlags">FMFD_DEFAULT (0x00000000) No flags specified.Use default behavior for the function. FMFD_RETURNUPDATEDIMGMIMES (0x00000020) Internet Explorer 9. Returns image/png and image/jpeg instead of image/x-png and image/pjpeg.</param>
		/// <param name="ppwzMimeOut">The address of a string value that receives the suggested MIME type.</param>
		/// <param name="dwReserved">Reserved. Must be set to 0.</param>
		[DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
		static extern int FindMimeFromData(
			IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)]  string pwzMimeProposed,
			int dwMimeFlags,
			out IntPtr ppwzMimeOut,
			int dwReserved);

		public string BuildPath(string[] directories, bool trimmed = false)
		{
			char directorySeparator = Path.DirectorySeparatorChar;
			string path = string.Join(directorySeparator, directories);
			if (trimmed == false) path += directorySeparator;
			return path;
		}

		public async Task<byte[]> GetFileBytesAsync(IFormFile formFile)
		{
			using (var memoryStream = new MemoryStream())
			{
				await formFile.CopyToAsync(memoryStream);
				byte[] fileBytes = memoryStream.ToArray();
				return fileBytes;
			}
		}

		public async Task<string> GetMimeTypeAsync(IFormFile formFile)
		{
			byte[] fileBytes = await GetFileBytesAsync(formFile);
			if (fileBytes.Length == 0) return null;
			int essentialMetaBytes = Math.Min(fileBytes.Length, 256);
			byte[] metaBytes = fileBytes.Take(essentialMetaBytes).ToArray();
			try
			{
				FindMimeFromData(
					pBC: IntPtr.Zero,
					pwzUrl: null,
					pBuffer: metaBytes,
					cbSize: metaBytes.Length,
					pwzMimeProposed: BinaryMimeType,
					dwMimeFlags: 0x00000020,
					ppwzMimeOut: out IntPtr mimeTypePointer,
					dwReserved: 0);
				string mimeType = Marshal.PtrToStringUni(mimeTypePointer);
				switch (mimeType)
				{
					case "image/pjpeg":
						mimeType = JpegMimeType;
						break;
					case "image/x-citrix-pjpeg":
						mimeType = JpegMimeType;
						break;
					case "image/x-png":
						mimeType = PngMimeType;
						break;
				}
				Marshal.FreeCoTaskMem(mimeTypePointer);
				return mimeType;
			}
			catch { return BinaryMimeType; }
		}

		public async Task<string> LoadTextFileContentAsync(string path)
		{
			path = (environment.ContentRootPath + path)
				.FormatPathSeparators();
			string content = await File.ReadAllTextAsync(path, Encoding.UTF8);
			return content;
		}

		public async Task SaveFileAsync(byte[] content)
		{
			await SaveFileAsync(content, environment.WebRootPath);
		}

		public async Task SaveFileAsync(byte[] content, string path)
		{
			if (content == null || content.Length == 0)
			{
				throw new ArgumentNullException(nameof(content));
			}
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException(nameof(path));
			}
			path = (environment.ContentRootPath + path)
				.FormatPathSeparators();
			EnsurePath(path);
			await File.WriteAllBytesAsync(path, content);
		}

		public void EnsurePath(string path)
		{
			string fileName = Path.GetFileName(path);
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				path = path.Replace(fileName, string.Empty);
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public async Task SaveFileAsync(IFormFile formFile)
		{
			await SaveFileAsync(formFile, environment.WebRootPath);
		}

		public async Task SaveFileAsync(IFormFile formFile, string path)
		{
			byte[] fileBytes = await GetFileBytesAsync(formFile);
			await SaveFileAsync(fileBytes, path);
		}
	}
}
