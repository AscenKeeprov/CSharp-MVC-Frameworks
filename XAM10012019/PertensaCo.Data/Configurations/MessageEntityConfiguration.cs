using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.DBConstants;
using static PertensaCo.Common.Constants.HTTPConstants;
using static PertensaCo.Common.Constants.MessageConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Data.Configurations
{
    public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
    {
	private bool SenderIsUnicode => EmailIsUnicode || UsernameIsUnicode;
	private int SenderMaxLength => Math
	    .Max(EmailMaxLength, Math.Max(UsernameMaxLength, PhoneNumberMaxLength));

	public void Configure(EntityTypeBuilder<Message> entityBuilder)
	{
	    entityBuilder.HasKey(m => m.Id);

	    entityBuilder.HasIndex(m => m.DateSent);

	    entityBuilder.HasIndex(m => m.Sender);

	    entityBuilder.Property(m => m.Content)
		.IsUnicode(true)
		.HasMaxLength(ContentMaxLength);

	    entityBuilder.Property(m => m.ContentType)
		.IsUnicode(false)
		.HasMaxLength(MimeTypeMaxLength);

	    entityBuilder.Property(m => m.DateReceived)
		.HasColumnType(DateTimeUtcNoTicksType);

	    entityBuilder.Property(m => m.DateSent)
		.HasColumnType(DateTimeUtcNoTicksType)
		.HasDefaultValueSql(DateTimeUtcFunc);

	    entityBuilder.Property(m => m.Sender)
		.IsRequired(true)
		.IsUnicode(false)
		.HasMaxLength(SenderMaxLength);

	    entityBuilder.Property(m => m.Subject)
		.IsRequired(true)
		.IsUnicode(true)
		.HasMaxLength(SubjectMaxLength);

	    entityBuilder.HasMany(m => m.Attachments)
		.WithOne(a => a.Container);
	}
    }
}
