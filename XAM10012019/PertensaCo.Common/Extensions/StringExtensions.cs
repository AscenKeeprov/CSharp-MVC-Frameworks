using System;
using System.IO;
using System.Text;

namespace PertensaCo.Common.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Ensures that all directory separators in the provided path string are unified.
		/// <para>Current execution platform is taken into account.</para>
		/// </summary>
		public static string FormatPathSeparators(this string path)
		{
			var directorySeparator = Path.DirectorySeparatorChar.ToString();
			while (path.Contains(@"//"))
			{
				path = path.Replace(@"//", directorySeparator);
			}
			path = path.Replace(@"/", directorySeparator);
			while (path.Contains(@"\\"))
			{
				path = path.Replace(@"\\", directorySeparator);
			}
			path = path.Replace(@"\", directorySeparator);
			return path;
		}

		public static string FromBase64(this string input)
		{
			byte[] inputBytes = Convert.FromBase64String(input);
			string output = Encoding.UTF8.GetString(inputBytes);
			return output;
		}

		public static string ToBase64(this string input)
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(input);
			string output = Convert.ToBase64String(inputBytes);
			return output;
		}
	}
}
