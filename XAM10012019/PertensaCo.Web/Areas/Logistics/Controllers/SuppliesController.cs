using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PertensaCo.Common.Extensions;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Logistics.Models;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Filters;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.MVCConstants;

namespace PertensaCo.Web.Areas.Logistics.Controllers
{
	[Authorize(Policy = LogisticsAuthorizationPolicyName)]
	[Area("Logistics")]
	public class SuppliesController : Controller
	{
		private readonly IEmployeesService employeeService;
		private readonly ILogger<SuppliesController> logger;
		private readonly IMaterialsService materialsService;

		public SuppliesController(IMaterialsService materialsService,
			IEmployeesService employeeService, ILogger<SuppliesController> logger)
		{
			this.materialsService = materialsService;
			this.employeeService = employeeService;
			this.logger = logger;
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult Check()
		{
			var materials = materialsService.GetMaterials();
			var model = new SupplyCheckViewModel()
			{
				Materials = materials.OrderBy(m => m.Element.ToString())
			};
			return View(model);
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult Procure([FromQuery(Name = "mid")] string materialId)
		{
			var materials = materialsService.GetMaterials();
			var suppliers = materialsService.GetSuppliers();
			var model = new MaterialProcureViewModel()
			{
				MaterialId = materialId,
				Materials = materials.Select(m => new SelectListItem()
				{
					Text = $"{m.Element.GetDisplayName()} ({m.Form.ToString()})",
					Value = m.Id.ToString()
				}).OrderBy(i => i.Text),
				Suppliers = suppliers.Where(s => s.ToString() != "None").ToArray()
			};
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[ValidateModelStateFilter]
		[HttpPost(ControllerActionRouteTemplate)]
		public async Task<IActionResult> Procure(MaterialProcureViewModel model)
		{
			var materials = materialsService.GetMaterials();
			var suppliers = materialsService.GetSuppliers();
			model.Materials = materials.Select(m => new SelectListItem()
			{
				Text = $"{m.Element.GetDisplayName()} ({m.Form.ToString()})",
				Value = m.Id.ToString()
			}).OrderBy(i => i.Text);
			model.Suppliers = suppliers.Where(s => s.ToString() != "None").ToArray();
			if (string.IsNullOrWhiteSpace(model.MaterialId))
			{
				ViewData[ErrorKey] = string.Format(SelectionNullErrorMessage, "a material");
				return View(model);
			}
			if (model.Quantity <= 0)
			{
				ViewData[ErrorKey] = "Quantity must be positive.";
				return View(model);
			}
			var material = await materialsService.FindByIdAsync(model.MaterialId);
			if (material == null)
			{
				ViewData[ErrorKey] = "No such material found in inventory list.";
				return View(model);
			}
			int materialRarity = (int)material.Element;
			int supplierDaysToDelivery = int.Parse(model.SupplierSpeed);
			if (materialRarity > 26 && supplierDaysToDelivery >= 15
				|| materialRarity > 42 && supplierDaysToDelivery >= 9)
			{
				ViewData[ErrorKey] = "This supplier does not have the required quantity. Try another.";
				return View(model);
			}
			material.QuantityInKg += model.Quantity;
			materialsService.UpdateStock(material);
			string materialDisplayName = material.Element.GetDisplayName().ToLower() +
				$" {material.Form.ToString().ToLower()}";
			ViewData[InfoKey] = $"{model.Quantity}kg of {materialDisplayName} added to inventory.";
			await employeeService.UpdateLastWorkedAsync(User.GetId());
			return View(model);
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public async Task<IActionResult> UpdatePrice(
			[FromQuery(Name = "mid")]string materialId,
			[FromQuery(Name = "qty")]string quantity)
		{
			if (string.IsNullOrWhiteSpace(materialId)
				|| string.IsNullOrWhiteSpace(quantity))
			{
				return Json(new { Price = "--" });
			}
			else
			{
				var material = await materialsService.FindByIdAsync(materialId);
				if (material == null) return Json(new { Price = "--" });
				double quantityInKg = Math.Abs(double.Parse(quantity));
				decimal price = (decimal)quantityInKg * material.PricePerKgInEur;
				return Json(new { Price = $"{price:F2}" });
			}
		}
	}
}
