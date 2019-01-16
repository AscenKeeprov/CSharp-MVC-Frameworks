using Chushka.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chushka.Data.EntityConfiguration
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
	public void Configure(EntityTypeBuilder<Order> entityBuilder)
	{
	    entityBuilder.HasKey(o => o.Id);

	    entityBuilder.HasOne(o => o.Client)
		.WithMany(u => u.Orders)
		.HasForeignKey(o => o.ClientId)
		.OnDelete(DeleteBehavior.Restrict);

	    entityBuilder.HasOne(o => o.Product)
		.WithMany(p => p.Orders)
		.HasForeignKey(o => o.ProductId)
		.OnDelete(DeleteBehavior.Restrict);

	    entityBuilder.Property(o => o.Id)
		.UseSqlServerIdentityColumn();

	    entityBuilder.Property(o => o.OrderedOn)
		.IsRequired(true)
		.HasDefaultValueSql("GETUTCDATE()");
	}
    }
}
