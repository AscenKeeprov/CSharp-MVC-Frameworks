using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.HTTPConstants;

namespace PertensaCo.Data.Configurations
{
	public class AttachmentEntityConfiguration : IEntityTypeConfiguration<Attachment>
	{
		public void Configure(EntityTypeBuilder<Attachment> entityBuilder)
		{
			entityBuilder.ToTable($"{nameof(Attachment)}s");

			entityBuilder.HasKey(a => a.Id);

			entityBuilder.Property(a => a.Name)
				.IsRequired(true)
				.HasMaxLength(FileNameMaxLength);

			entityBuilder.Property(a => a.Type)
				.IsUnicode(false)
				.HasMaxLength(MimeTypeMaxLength);

			entityBuilder.Property(a => a.Url)
				.IsRequired(true)
				.IsUnicode(false)
				.HasMaxLength(UrlMaxLength);

			entityBuilder.HasOne(a => a.Container)
				.WithMany(c => c.Attachments);
		}
	}
}
