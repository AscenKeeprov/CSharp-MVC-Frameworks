using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Data.Configurations
{
	public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
	{
		public void Configure(EntityTypeBuilder<Client> entityBuilder)
		{
			entityBuilder.HasKey(c => c.Id);

			entityBuilder.Property(c => c.CompanyName)
				.HasMaxLength(CompanyNameMaxLength);

			entityBuilder.Property(c => c.FirstName)
				.IsRequired(false)
				.HasMaxLength(PersonalNameMaxLength);

			entityBuilder.Property(c => c.LastName)
				.IsRequired(false)
				.HasMaxLength(PersonalNameMaxLength);

			entityBuilder.Property(c => c.ShippingAddress)
				.HasMaxLength(AddressMaxLength);

			entityBuilder.HasOne(c => c.Profile)
				.WithOne(u => u.Client)
				.HasPrincipalKey<User>(u => u.Id);
		}
	}
}
