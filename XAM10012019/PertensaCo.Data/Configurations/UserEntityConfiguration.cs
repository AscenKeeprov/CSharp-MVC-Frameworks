using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.DBConstants;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Data.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
	public void Configure(EntityTypeBuilder<User> entityBuilder)
	{
	    entityBuilder.ToTable($"{nameof(User)}s");

	    entityBuilder.HasKey(u => u.Id);

	    entityBuilder.HasIndex(u => u.NormalizedEmail)
		.IsUnique(EmailIsUnique);

	    entityBuilder.HasIndex(u => u.NormalizedUserName)
		.IsUnique(UsernameIsUnique);

	    entityBuilder.Property(u => u.AccessFailedCount)
		.HasColumnType(TinyIntType);

	    entityBuilder.Property(u => u.ConcurrencyStamp)
		.IsConcurrencyToken(true)
		.IsUnicode(false)
		.IsFixedLength(true)
		.HasMaxLength(GuidStringLength);

	    entityBuilder.Property(u => u.DateDeleted)
		.HasColumnType(DateTimeUtcNoTicksType);

	    entityBuilder.Property(u => u.DateLastLogin)
		.HasColumnType(DateTimeUtcNoTicksType);

	    entityBuilder.Property(u => u.DateRegistered)
		.HasColumnType(DateTimeUtcNoTicksType)
		.HasDefaultValueSql(DateTimeUtcFunc);

	    entityBuilder.Property(u => u.Email)
		.IsRequired(true)
		.IsUnicode(EmailIsUnicode)
		.HasMaxLength(EmailMaxLength);

	    entityBuilder.Property(u => u.Id)
		.IsUnicode(false)
		.IsFixedLength(true)
		.HasMaxLength(GuidStringLength);

	    entityBuilder.Property(u => u.LockoutEnd)
		.HasColumnType(DateTimeUtcNoTicksType);

	    entityBuilder.Property(u => u.NormalizedEmail)
		.IsUnicode(false)
		.HasMaxLength(EmailMaxLength);

	    entityBuilder.Property(u => u.NormalizedUserName)
		.IsUnicode(false)
		.HasColumnName(NormalizedAliasColumnName)
		.HasMaxLength(UsernameMaxLength);

	    entityBuilder.Property(u => u.PasswordHash)
		.IsRequired(true)
		.IsUnicode(false)
		.HasMaxLength(PasswordHashMaxLength);

	    entityBuilder.Property(u => u.PhoneNumber)
		.IsUnicode(false)
		.HasMaxLength(PhoneNumberMaxLength);

	    entityBuilder.Property(u => u.SecurityStamp)
		.IsUnicode(false)
		.IsFixedLength(true)
		.HasMaxLength(SecurityStampMaxLength);

	    entityBuilder.Property(u => u.UserName)
		.IsRequired(true)
		.IsUnicode(UsernameIsUnicode)
		.HasColumnName(AliasColumnName)
		.HasMaxLength(UsernameMaxLength);

	    entityBuilder.HasMany(u => u.Claims)
		.WithOne(uc => uc.User)
		.HasForeignKey(uc => uc.UserId);

	    entityBuilder.HasMany(u => u.Logins)
		.WithOne(ul => ul.User)
		.HasForeignKey(ul => ul.UserId);

	    entityBuilder.HasMany(u => u.Roles)
		.WithOne(ur => ur.User)
		.HasForeignKey(ur => ur.UserId);

	    entityBuilder.HasMany(u => u.Tokens)
		.WithOne(ut => ut.User)
		.HasForeignKey(ut => ut.UserId);

	    entityBuilder.HasOne(u => u.Client)
		.WithOne(c => c.Profile)
		.HasPrincipalKey<User>(u => u.Id);

	    entityBuilder.HasOne(u => u.Employee)
		.WithOne(e => e.Profile)
		.HasPrincipalKey<User>(u => u.Id);
	}
    }
}
