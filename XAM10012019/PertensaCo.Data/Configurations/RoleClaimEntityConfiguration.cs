using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;

namespace PertensaCo.Data.Configurations
{
    public class RoleClaimEntityConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
	public void Configure(EntityTypeBuilder<RoleClaim> entityBuilder)
	{
	    entityBuilder.ToTable($"{nameof(Role)}{nameof(Claim)}s");

	    entityBuilder.HasKey(rc => rc.Id);

	    entityBuilder.HasOne(rc => rc.Role)
		.WithMany(r => r.Claims)
		.HasForeignKey(rc => rc.RoleId);
	}
    }
}
