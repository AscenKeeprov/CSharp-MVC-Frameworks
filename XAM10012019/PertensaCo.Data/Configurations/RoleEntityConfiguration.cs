using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.DBConstants;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.RoleConstants;

namespace PertensaCo.Data.Configurations
{
	public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> entityBuilder)
		{
			entityBuilder.ToTable($"{nameof(Role)}s");

			entityBuilder.HasKey(r => r.Id);

			entityBuilder.HasIndex(r => r.NormalizedName)
				.IsUnique(true);

			entityBuilder.Property(r => r.ConcurrencyStamp)
				.IsConcurrencyToken(true)
				.IsUnicode(false)
				.IsFixedLength(true)
				.HasMaxLength(GuidStringLength);

			entityBuilder.Property(e => e.Department)
				.HasColumnType(TinyIntType);

			entityBuilder.Property(r => r.Id)
				.IsUnicode(false)
				.IsFixedLength(true)
				.HasMaxLength(GuidStringLength);

			entityBuilder.Property(r => r.DisplayName)
				.HasMaxLength(RoleDisplayNameMaxLength);

			entityBuilder.Property(r => r.Name)
				.IsRequired(true)
				.IsUnicode(false)
				.HasMaxLength(RoleNameMaxLength);

			entityBuilder.Property(r => r.NormalizedName)
				.IsUnicode(false)
				.HasMaxLength(RoleNameMaxLength);

			entityBuilder.HasMany(r => r.Claims)
				.WithOne(rc => rc.Role)
				.HasForeignKey(rc => rc.RoleId);

			entityBuilder.HasMany(r => r.Employees)
				.WithOne(e => e.Position);

			entityBuilder.HasMany(r => r.Users)
				.WithOne(ur => ur.Role)
				.HasForeignKey(ur => ur.RoleId);
		}
	}
}
