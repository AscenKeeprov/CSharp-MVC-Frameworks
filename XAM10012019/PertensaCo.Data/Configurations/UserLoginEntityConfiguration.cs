using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Data.Configurations
{
    public class UserLoginEntityConfiguration : IEntityTypeConfiguration<UserLogin>
    {
	public void Configure(EntityTypeBuilder<UserLogin> entityBuilder)
	{
	    entityBuilder.ToTable($"{nameof(User)}Logins");

	    entityBuilder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

	    entityBuilder.Property(ul => ul.LoginProvider)
		.HasMaxLength(LoginProviderMaxLength);

	    entityBuilder.Property(ul => ul.ProviderDisplayName)
		.HasMaxLength(LoginProviderNameMaxLength);

	    entityBuilder.Property(ul => ul.ProviderKey)
		.HasMaxLength(LoginProviderKeyMaxLength);

	    entityBuilder.HasOne(ul => ul.User)
		.WithMany(u => u.Logins)
		.HasForeignKey(ul => ul.UserId);
	}
    }
}
