using FDMC.Web.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FDMC.Web.Data.EntityConfiguration
{
    public class CatEntityConfiguration : IEntityTypeConfiguration<Cat>
    {
	public void Configure(EntityTypeBuilder<Cat> entityBuilder)
	{
	    entityBuilder.HasIndex(c => new { c.Name, c.Age, c.Breed })
		.IsUnique(true);
	}
    }
}
