using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;

namespace PertensaCo.Data.Configurations
{
    public class UserClaimEntityConfiguration : IEntityTypeConfiguration<UserClaim>
    {
	public void Configure(EntityTypeBuilder<UserClaim> entityBuilder)
	{
	    entityBuilder.ToTable($"{nameof(User)}{nameof(Claim)}s");

	    entityBuilder.HasKey(uc => uc.Id);

	    entityBuilder.HasOne(uc => uc.User)
		.WithMany(u => u.Claims)
		.HasForeignKey(uc => uc.UserId);
	}
    }
}
