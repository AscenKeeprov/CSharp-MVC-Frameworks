using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PertensaCo.Common.Extensions;
using PertensaCo.Entities.Attributes;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Innovation.Models;
using PertensaCo.Web.DataTransferObjects;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Filters;
using static PertensaCo.Common.Constants.AlloyConstants;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.HTMLConstants;
using static PertensaCo.Common.Constants.MVCConstants;

namespace PertensaCo.Web.Areas.Innovation.Controllers
{
	[Area("Innovation")]
	[Authorize(Policy = ScientistsAuthorizationPolicyName)]
	public class ResearchController : Controller
	{
		private readonly IEmployeesService employeeService;
		private readonly ILogger<ResearchController> logger;
		private readonly IMaterialsService materialsService;

		public ResearchController(IMaterialsService materialsService,
			IEmployeesService employeeService, ILogger<ResearchController> logger)
		{
			this.employeeService = employeeService;
			this.materialsService = materialsService;
			this.logger = logger;
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult Conduct()
		{
			var materials = materialsService.GetMaterials();
			var model = new ResearchViewModel()
			{
				AdditionalMaterials = materials.Where(
					m => m.Element.HasAttribute<UsageAttribute>()
					&& m.Element.GetAttribute<UsageAttribute>().IsUsedAsAdditive)
					.Select(m => new SelectListItem()
					{
						Disabled = m.QuantityInKg > 0 ? false : true,
						Text = $"{m.Element.GetDisplayName()} ({m.Form.ToString()})"
						+ (m.QuantityInKg > 0 ? string.Empty : " [DEPLETED]"),
						Value = $"{m.Element.ToString()}_{(int)m.Form}"
					}).OrderBy(i => i.Text),
				BaseMaterials = materials.Where(
					m => m.Element.HasAttribute<UsageAttribute>()
					&& m.Element.GetAttribute<UsageAttribute>().IsUsedAsBase)
					.Select(m => new SelectListItem()
					{
						Disabled = m.QuantityInKg > 0 ? false : true,
						Text = $"{m.Element.GetDisplayName()} ({m.Form.ToString()})"
						+ (m.QuantityInKg > 0 ? string.Empty : " [DEPLETED]"),
						Value = $"{m.Element.ToString()}_{(int)m.Form}"
					}).OrderBy(i => i.Text)
			};
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[ValidateModelStateFilter]
		[HttpPost(ControllerActionRouteTemplate)]
		public async Task<IActionResult> Conduct([FromForm(Name = "materials")]string data)
		{
			var materialDtos = JsonConvert.DeserializeObject<MaterialDTO[]>(data);
			if (materialDtos.Length < 2) return BadRequest(new
			{
				Message = "You must add more materials to make an alloy."
			});
			if (materialDtos.Length > 14) return BadRequest(new { Message = "Failure!" });
			var baseMaterial = materialDtos.FirstOrDefault(m
				=> m.Type.Equals("base", StringComparison.InvariantCultureIgnoreCase));
			if (baseMaterial == null) return BadRequest(new
			{
				Message = string.Format(SelectionNullErrorMessage, "a base material")
			});
			double totalRequiredMaterialsQuantity = materialDtos.Sum(m => m.Quantity);
			var alloyComposition = new Dictionary<string, double>();
			var requiredMaterials = new (string Element, int Form, double Quantity)[0];
			decimal researchCost = 0M;
			foreach (var materialDto in materialDtos)
			{
				string elementName = materialDto.Element.ToString();
				int formCode = (int)materialDto.Form;
				var material = materialsService.GetMaterial(elementName, formCode);
				if (material.QuantityInKg < materialDto.Quantity)
				{
					var missingMaterialName = materialDto.Element.GetDisplayName()
						+ $" ({materialDto.Form.GetDisplayName()})";
					return BadRequest(new
					{
						Message = string.Join(HtmlNewLine, new string[]
						{
							$"Insufficient quantities of {missingMaterialName.ToLower()}.",
							"Refresh page to get the latest inventoty information."
						})
					});
				}
				double materialPercentage = materialDto.Quantity / totalRequiredMaterialsQuantity * 100;
				bool isBaseMaterial = materialDto.Type.Equals("base", StringComparison.InvariantCultureIgnoreCase);
				if (isBaseMaterial && materialPercentage < BaseMaterialMinPercentage) return BadRequest(new
				{
					Message = $"Base should comprise at least {BaseMaterialMinPercentage}% of the alloy."
				});
				if (!alloyComposition.ContainsKey(elementName))
				{
					alloyComposition.Add(elementName, materialPercentage);
				}
				else alloyComposition[elementName] += materialPercentage;
				var requiredMaterial = ValueTuple.Create(elementName, formCode, materialDto.Quantity);
				requiredMaterials = requiredMaterials.Append(requiredMaterial).ToArray();
				researchCost += (decimal)materialDto.Quantity * material.PricePerKgInEur;
			}
			try
			{
				var alloy = await materialsService.CreateAlloyAsync(alloyComposition, researchCost);
				materialsService.ExpendMaterials(requiredMaterials);
				await employeeService.UpdateLastWorkedAsync(User.GetId());
				return Json(new
				{
					Message = string.Join(HtmlNewLine, new string[]
					{
						$"Success! You have created a new alloy: [{alloy.Composition}]",
						$"Total cost of the research project: €{researchCost:F2}"
					})
				});
			}
			catch (Exception exception)
			{
				logger.LogError(exception.GetMessageStack());
				return BadRequest(new { Message = exception.Message });
			}
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult GetMaterialReserves([FromQuery] string elementName, [FromQuery] string formCode)
		{
			if (string.IsNullOrWhiteSpace(elementName) || string.IsNullOrWhiteSpace(formCode))
			{
				return BadRequest(new
				{
					Message = string.Format(InvalidObjectErrorMessage, "Data")
				});
			}
			var material = materialsService.GetMaterial(elementName, int.Parse(formCode));
			if (material == null) return BadRequest(new { Message = "Unknown material" });
			var reserves = material.QuantityInKg;
			return Json(new { Reserves = reserves });
		}
	}
}
