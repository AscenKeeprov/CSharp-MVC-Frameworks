using System.Threading.Tasks;

namespace PertensaCo.Services.Contracts
{
	public interface IDatabaseService
	{
		Task InitializeDatabaseAsync();
		Task SeedDatabaseAsync();
	}
}
