using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.AlloyConstants;
using static PertensaCo.Common.Constants.DBConstants;

namespace PertensaCo.Data.Configurations
{
	public class AlloyEntityConfiguration : IEntityTypeConfiguration<Alloy>
	{
		public void Configure(EntityTypeBuilder<Alloy> entityBuilder)
		{
			entityBuilder.HasKey(a => a.Id);

			entityBuilder.HasIndex(a => a.Composition)
				.IsUnique(true);

			entityBuilder.Property(a => a.Composition)
				.IsRequired(true)
				.IsUnicode(false)
				.HasMaxLength(CompositionMaxLength);

			entityBuilder.Property(a => a.ResearchCost)
				.IsRequired(true)
				.HasColumnType(DecimalPrecision16Scale2Type)
				.HasDefaultValue(default(decimal));
		}
	}
}
