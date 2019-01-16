using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PertensaCo.Data;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services;
using Xunit;

namespace PertensaCo.Tests.PersonnelAreaTests
{
	public class EmployeesServiceTests : IDisposable
	{
		PertensaDbContext inMemoryDbContext;

		public EmployeesServiceTests()
		{
			var dbContextOptionsBuilder = new DbContextOptionsBuilder<PertensaDbContext>();
			dbContextOptionsBuilder.UseInMemoryDatabase("MockPertensaDB");
			inMemoryDbContext = new PertensaDbContext(dbContextOptionsBuilder.Options);
			SeedMockDatabase(inMemoryDbContext);
		}

		private void SeedMockDatabase(PertensaDbContext inMemoryDbContext)
		{
			inMemoryDbContext.Employees.AddRangeAsync(new Employee[]
			{
				new Employee(){
					FirstName = "First", MiddleName = "Middle", LastName = "Last"
				},
			});
			inMemoryDbContext.SaveChanges();
		}

		public void Dispose() { }

		[Fact]
		public async Task Hire_ReturnsNull_WhenEmployeeDataIsIncorrect()
		{
			#region ARRANGE
			var employeesService = new EmployeesService(inMemoryDbContext);
			Employee employee = null;
			#endregion
			#region ACT
			var expectedException = new ArgumentNullException();
			try
			{
				await employeesService.HireAsync(employee);
			}
			catch (Exception exception)
			{
				expectedException = exception as ArgumentNullException;
			}
			#endregion
			#region ASSERT
			Assert.IsAssignableFrom<Exception>(expectedException);
			Assert.NotNull(expectedException);
			Assert.IsType<ArgumentNullException>(expectedException);
			#endregion
		}

		[Fact]
		public async Task Hire_SavesEmployeeData_WhenEmployeeDataIsCorrect()
		{
			#region ARRANGE
			var employeesService = new EmployeesService(inMemoryDbContext);
			Employee employee = new Employee()
			{
				FirstName = "First",
				MiddleName = "Middle",
				LastName = "Last Record",
				Department = EDepartment.IT,
				HomeAddress = "Home",
				MonthlySalaryInEUR = 1234M,
				WorkAddress = "Office"
			};
			var initialRecordsCount = inMemoryDbContext.Employees.Count();
			#endregion
			#region ACT
			await employeesService.HireAsync(employee);
			var finalRecordsCount = inMemoryDbContext.Employees.Count();
			var lastRecord = await inMemoryDbContext.Employees.LastOrDefaultAsync();
			#endregion
			#region ASSERT
			Assert.True(finalRecordsCount > initialRecordsCount);
			Assert.NotNull(lastRecord);
			Assert.Equal(lastRecord.LastName, employee.LastName);
			#endregion
		}

		[Fact]
		public async Task UpdateLastWorked_DoesNotAlterEmployeeData_WhenEmployeeNotFound()
		{
			#region ARRANGE
			var employeesService = new EmployeesService(inMemoryDbContext);
			var initialLastWorkedDates = inMemoryDbContext.Employees
				.Select(e => e.DateLastWorked).ToArray();
			#endregion
			#region ACT
			await employeesService.UpdateLastWorkedAsync("INEXISTENTID");
			var finalLastWorkedDates = inMemoryDbContext.Employees
				.Select(e => e.DateLastWorked).ToArray();
			#endregion
			#region ASSERT
			Assert.Equal(finalLastWorkedDates, initialLastWorkedDates);
			#endregion
		}
	}
}
