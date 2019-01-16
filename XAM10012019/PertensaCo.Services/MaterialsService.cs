using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PertensaCo.Common.Exceptions;
using PertensaCo.Data;
using PertensaCo.Data.Extensions;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;

namespace PertensaCo.Services
{
	public class MaterialsService : IMaterialsService
	{
		private readonly PertensaDbContext dbContext;

		public MaterialsService(PertensaDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public double CheckAvailability(string elementName, string formName)
		{
			var availableMaterial = GetMaterial(elementName, formName);
			if (availableMaterial == null) return 0;
			return availableMaterial.QuantityInKg;
		}

		public async Task<Alloy> CreateAlloyAsync(IEnumerable<KeyValuePair<string, double>> elements)
		{
			StringBuilder compositionInfo = new StringBuilder();
			foreach (var element in elements
				.OrderByDescending(kvp => kvp.Value)
				.ThenByDescending(kvp => kvp.Key))
			{
				compositionInfo.Append(element.Key);
				compositionInfo.Append($"{element.Value:F1}");
			}
			string composition = compositionInfo.ToString().Trim();
			if (dbContext.Alloys.Any(a => a.Composition == composition))
			{
				throw new DuplicateObjectException(typeof(Alloy), composition);
			}
			var alloy = new Alloy(composition);
			await dbContext.Alloys.AddAsync(alloy);
			dbContext.SaveChanges();
			return alloy;
		}

		public async Task<Alloy> CreateAlloyAsync(IEnumerable<KeyValuePair<string, double>> elements, decimal researchCost)
		{
			var alloy = await CreateAlloyAsync(elements);
			alloy.ResearchCost = researchCost;
			dbContext.Alloys.Update(alloy);
			dbContext.SaveChanges();
			return alloy;
		}

		public async Task<Alloy> CreateAlloyAsync(IEnumerable<(string Element, double Percentage)> constituents)
		{
			var composition = new Dictionary<string, double>();
			foreach (var constituent in constituents)
			{
				if (!composition.ContainsKey(constituent.Element))
				{
					composition.Add(constituent.Element, constituent.Percentage);
				}
				else composition[constituent.Element] += constituent.Percentage;
			}
			return await CreateAlloyAsync(composition);
		}

		public void ExpendMaterials((string Element, int Form, double Quantity)[] materials)
		{
			foreach (var material in materials)
			{
				var reserve = GetMaterial(material.Element, material.Form);
				if (reserve.QuantityInKg < material.Quantity) reserve.QuantityInKg = 0;
				else reserve.QuantityInKg -= material.Quantity;
				dbContext.Warehouse.Update(reserve);
			}
			dbContext.SaveChanges();
		}

		public async Task<Material> FindByIdAsync(string id)
		{
			Type materialPrimaryKeyType = dbContext
				.GetEntityPrimaryKeyType(typeof(Material));
			var materialId = Convert.ChangeType(id, materialPrimaryKeyType);
			var material = await dbContext.Warehouse.FindAsync(materialId);
			return material;
		}

		public Material GetMaterial(string elementName, string formName)
		{
			var materials = GetMaterials();
			Material material = materials.FirstOrDefault(m
				=> m.Element.ToString() == elementName
				&& m.Form.ToString() == formName);
			return material;
		}

		public Material GetMaterial(string elementName, int formCode)
		{
			var materials = GetMaterials();
			Material material = materials.FirstOrDefault(m
				=> m.Element.ToString() == elementName
				&& (int)m.Form == formCode);
			return material;
		}

		public Material GetMaterial(int elementCode, string formName)
		{
			var materials = GetMaterials();
			Material material = materials.FirstOrDefault(m
				=> (int)m.Element == elementCode
				&& m.Form.ToString() == formName);
			return material;
		}

		public Material GetMaterial(int elementCode, int formCode)
		{
			var materials = GetMaterials();
			Material material = materials.FirstOrDefault(m
				=> (int)m.Element == elementCode
				&& (int)m.Form == formCode);
			return material;
		}

		public IEnumerable<Material> GetMaterials()
		{
			var materials = dbContext.Warehouse.AsEnumerable();
			return materials;
		}

		public IEnumerable<ESupplier> GetSuppliers()
		{
			var suppliers = Enum.GetValues(typeof(ESupplier)).Cast<ESupplier>();
			return suppliers;
		}

		public async Task StockAsync(Material material)
		{
			await dbContext.Warehouse.AddAsync(material);
			dbContext.SaveChanges();
		}

		public void UpdateStock(Material newStock)
		{
			string element = newStock.Element.ToString();
			string form = newStock.Form.ToString();
			var oldStock = GetMaterial(element, form);
			oldStock.QuantityInKg = newStock.QuantityInKg;
			dbContext.Warehouse.Update(oldStock);
			dbContext.SaveChanges();
		}

		public void UpdateStock(Material oldStock, Material newStock)
		{
			oldStock.QuantityInKg = newStock.QuantityInKg;
			dbContext.Warehouse.Update(oldStock);
			dbContext.SaveChanges();
		}
	}
}
