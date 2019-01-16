using System.Collections.Generic;
using System.Threading.Tasks;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;

namespace PertensaCo.Services.Contracts
{
	public interface IMaterialsService
	{
		double CheckAvailability(string elementName, string formName);
		Task<Alloy> CreateAlloyAsync(IEnumerable<KeyValuePair<string, double>> alloyComposition);
		Task<Alloy> CreateAlloyAsync(IEnumerable<KeyValuePair<string, double>> alloyComposition, decimal researchCost);
		Task<Alloy> CreateAlloyAsync(IEnumerable<(string Element, double Percentage)> alloyComposition);
		void ExpendMaterials((string Element, int Form, double Quantity)[] materials);
		Task<Material> FindByIdAsync(string materialId);
		Material GetMaterial(string elementName, string formName);
		Material GetMaterial(string elementName, int formCode);
		Material GetMaterial(int elementCode, string formName);
		Material GetMaterial(int elementCode, int formCode);
		IEnumerable<Material> GetMaterials();
		IEnumerable<ESupplier> GetSuppliers();
		Task StockAsync(Material material);
		void UpdateStock(Material newStock);
		void UpdateStock(Material oldStock, Material newStock);
	}
}
