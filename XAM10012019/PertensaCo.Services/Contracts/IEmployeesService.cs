using System.Collections.Generic;
using System.Threading.Tasks;
using PertensaCo.Entities;

namespace PertensaCo.Services.Contracts
{
	public interface IEmployeesService
	{
		Task<Employee> FindByIdAsync(string employeeId);
		Employee FindByPin(string personalIdentificationNumber);
		IEnumerable<Employee> GetEmployees();
		Task HireAsync(Employee employee);
		Task UpdateLastWorkedAsync(string profileId);
	}
}
