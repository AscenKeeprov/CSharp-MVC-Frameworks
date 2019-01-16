using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.DBConstants;

namespace PertensaCo.Data.Configurations
{
	public class MaterialEntityConfiguration : IEntityTypeConfiguration<Material>
	{
		public void Configure(EntityTypeBuilder<Material> entityBuilder)
		{
			entityBuilder.HasKey(m => m.Id);

			entityBuilder.HasIndex(m => new { m.Element, m.Form })
				.IsUnique(true);

			entityBuilder.Property(m => m.Element)
				.IsRequired(true)
				.HasColumnType(TinyIntType);

			entityBuilder.Property(m => m.Form)
				.IsRequired(true)
				.HasColumnType(TinyIntType);

			entityBuilder.Property(m => m.PricePerKgInEur)
				.HasColumnType(DecimalPrecision9Scale2Type);

			entityBuilder.Property(m => m.QuantityInKg)
				.HasColumnType(NumericPrecision9Scale3Type)
				.HasDefaultValue(default(double));
		}
	}
}
