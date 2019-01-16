using System;
using static PertensaCo.Common.Constants.HTTPConstants;

namespace PertensaCo.Entities
{
	public class Attachment : Entity<Guid>
	{
		public Attachment(string name, string url)
		{
			Name = name;
			Url = url;
		}

		public string Name { get; set; }

		/// <summary>The size of the file in bytes.</summary>
		public uint SizeInBytes { get; set; }

		/// <summary>The MIME type of the file.</summary>
		public string Type { get; set; } = BinaryMimeType;

		/// <summary>The location of the file on the server.</summary>
		public string Url { get; set; }

		/// <summary>What the file is attached to.</summary>
		public virtual Message Container { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
