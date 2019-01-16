using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PertensaCo.Data;
using PertensaCo.Data.Extensions;
using PertensaCo.Entities;
using PertensaCo.Services.Contracts;

namespace PertensaCo.Services
{
	public class EmployeesService : IEmployeesService
	{
		private readonly PertensaDbContext dbContext;

		public EmployeesService(PertensaDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<Employee> FindByIdAsync(string id)
		{
			Type employeePrimaryKeyType = dbContext
				.GetEntityPrimaryKeyType(typeof(Employee));
			var employeeId = Convert.ChangeType(id, employeePrimaryKeyType);
			var employee = await dbContext.Employees.FindAsync(employeeId);
			return employee;
		}

		public Employee FindByPin(string personalIdentificationNumber)
		{
			var employee = dbContext.Employees
				.FirstOrDefault(e => e.PIN == personalIdentificationNumber);
			return employee;
		}

		public IEnumerable<Employee> GetEmployees()
		{
			var employees = dbContext.Employees.AsEnumerable();
			return employees;
		}

		public async Task HireAsync(Employee employee)
		{
			if (employee == null)
			{
				throw new ArgumentNullException(nameof(employee));
			}
			await dbContext.Employees.AddAsync(employee);
			dbContext.SaveChanges();
		}

		public async Task UpdateLastWorkedAsync(string id)
		{
			Type profilePrimaryKeyType = dbContext
				.GetEntityPrimaryKeyType(typeof(User));
			var profileId = Convert.ChangeType(id, profilePrimaryKeyType);
			var profile = await dbContext.Users.FindAsync(profileId);
			if (profile != null)
			{
				var employee = profile.Employee;
				if (employee != null)
				{
					employee.DateLastWorked = DateTimeOffset.Now;
					dbContext.SaveChanges();
				}
			}
		}
	}
}
