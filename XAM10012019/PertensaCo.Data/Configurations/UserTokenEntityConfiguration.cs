using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.DBConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Data.Configurations
{
    public class UserTokenEntityConfiguration : IEntityTypeConfiguration<UserToken>
    {
	public void Configure(EntityTypeBuilder<UserToken> entityBuilder)
	{
	    entityBuilder.ToTable($"{nameof(User)}Tokens");

	    entityBuilder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

	    entityBuilder.Property(ut => ut.LoginProvider)
		.HasMaxLength(LoginProviderMaxLength);

	    entityBuilder.Property(ut => ut.Name)
		.HasColumnName(TokenNameColumnName)
		.HasMaxLength(TokenNameMaxLength);

	    entityBuilder.Property(ut => ut.Value)
		.HasColumnName(TokenValueColumnName);

	    entityBuilder.HasOne(ut => ut.User)
		.WithMany(u => u.Tokens)
		.HasForeignKey(ut => ut.UserId);
	}
    }
}
