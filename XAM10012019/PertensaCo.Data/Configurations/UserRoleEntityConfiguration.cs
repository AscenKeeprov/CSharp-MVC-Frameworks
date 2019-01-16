using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;

namespace PertensaCo.Data.Configurations
{
    public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
    {
	public void Configure(EntityTypeBuilder<UserRole> entityBuilder)
	{
	    entityBuilder.ToTable($"{nameof(User)}{nameof(Role)}s");

	    entityBuilder.HasKey(ur => new { ur.UserId, ur.RoleId });

	    entityBuilder.HasIndex(ur => new { ur.UserId, ur.RoleId })
		.IsUnique(true);

	    entityBuilder.HasOne(ur => ur.User)
		.WithMany(u => u.Roles)
		.HasForeignKey(ur => ur.UserId)
		.OnDelete(DeleteBehavior.Restrict);

	    entityBuilder.HasOne(ur => ur.Role)
		.WithMany(r => r.Users)
		.HasForeignKey(ur => ur.RoleId)
		.OnDelete(DeleteBehavior.Restrict);
	}
    }
}
