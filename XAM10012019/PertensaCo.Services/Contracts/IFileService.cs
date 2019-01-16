using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PertensaCo.Services.Contracts
{
	public interface IFileService
	{
		/// <summary>
		/// Joins a set of directory names using the default directory separator for the current platform.
		/// <para>Unless specified otherwise, the resulting hierarchical path has a trailing separator.</para>
		/// </summary>
		string BuildPath(string[] directories, bool trimmed = false);

		/// <summary>Checks if any part of a path is missing and creates missing levels.</summary>
		void EnsurePath(string path);

		Task<byte[]> GetFileBytesAsync(IFormFile formFile);
		Task<string> GetMimeTypeAsync(IFormFile formFile);

		/// <summary>
		/// Reads a text file from a directory relative to the content root.
		/// <para>Uses UTF-8 encoding. Closes the file after reading it.</para>
		/// </summary>
		Task<string> LoadTextFileContentAsync(string path);

		Task SaveFileAsync(byte[] content);
		Task SaveFileAsync(IFormFile formFile);
		Task SaveFileAsync(byte[] content, string path);
		Task SaveFileAsync(IFormFile formFile, string path);
	}
}
