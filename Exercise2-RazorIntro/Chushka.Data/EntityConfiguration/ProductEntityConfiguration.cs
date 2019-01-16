using Chushka.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chushka.Data.EntityConfiguration
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
	public void Configure(EntityTypeBuilder<Product> entityBuilder)
	{
	    entityBuilder.HasKey(p => p.Id);

	    entityBuilder.HasMany(p => p.Orders)
		.WithOne(o => o.Product)
		.HasForeignKey(o => o.ProductId);

	    entityBuilder.Property(p => p.Name)
		.IsRequired(true);

	    entityBuilder.Property(p => p.Price)
		.IsRequired(true)
		.HasColumnType("DECIMAL(18,2)")
		.HasDefaultValue(0.01M);

	    entityBuilder.Property(p => p.Type)
		.IsRequired(true);
	}
    }
}
