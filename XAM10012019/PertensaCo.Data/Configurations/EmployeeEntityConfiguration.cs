using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.DBConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Data.Configurations
{
	public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
	{
		public void Configure(EntityTypeBuilder<Employee> entityBuilder)
		{
			entityBuilder.HasKey(e => e.Id);

			entityBuilder.HasIndex(e => e.PIN)
				.IsUnique(true);

			entityBuilder.Property(e => e.DateHired)
				.HasColumnType(DateType)
				.HasDefaultValueSql(DateFunc);

			entityBuilder.Property(e => e.DateLastWorked)
				.HasColumnType(DateTimeUtcNoTicksType);

			entityBuilder.Property(e => e.DateRelieved)
				.HasColumnType(DateType);

			entityBuilder.Property(e => e.Department)
				.IsRequired(true)
				.HasColumnType(TinyIntType);

			entityBuilder.Property(e => e.FirstName)
				.IsRequired(true)
				.HasMaxLength(PersonalNameMaxLength);

			entityBuilder.Property(e => e.HomeAddress)
				.IsRequired(true)
				.HasMaxLength(AddressMaxLength);

			entityBuilder.Property(e => e.LastName)
				.IsRequired(true)
				.HasMaxLength(PersonalNameMaxLength);

			entityBuilder.Property(e => e.MiddleName)
				.IsRequired(true)
				.HasMaxLength(PersonalNameMaxLength);

			entityBuilder.Property(e => e.PIN)
				.IsRequired(true)
				.IsUnicode(false)
				.HasMaxLength(PersonalIdentificationNumberMaxLength);

			entityBuilder.Property(e => e.Promotions)
				.HasDefaultValue(default(byte));

			entityBuilder.Property(e => e.MonthlySalaryInEUR)
				.HasColumnType(DecimalPrecision9Scale2Type)
				.IsRequired(true);

			entityBuilder.Property(e => e.WorkAddress)
				.IsRequired(true)
				.HasMaxLength(AddressMaxLength);

			entityBuilder.HasOne(e => e.Profile)
				.WithOne(u => u.Employee);

			entityBuilder.HasOne(e => e.Position)
				.WithMany(r => r.Employees);
		}
	}
}
