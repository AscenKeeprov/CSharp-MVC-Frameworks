using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Logistics.Models
{
	public class MaterialProcureViewModel : ViewModel
	{
		public MaterialProcureViewModel()
		{
			Materials = new List<SelectListItem>();
			Suppliers = new ESupplier[0];
		}

		[Display(Name = "Material")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string MaterialId { get; set; }

		public IEnumerable<SelectListItem> Materials { get; set; }

		[Display(Name = "Quantity (KG)")]
		[DisplayFormat(ApplyFormatInEditMode = true,
			DataFormatString = "{0:0.000}")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		[Range(5, 10000, ErrorMessage = RangeErrorMessage)]
		public double Quantity { get; set; }

		public override string PageTitle => "Restock supplies";

		[Display(Name = "Supplier")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string SupplierSpeed { get; set; }

		public ESupplier[] Suppliers { get; set; }

		[Display(Name = "Price (€)")]
		public decimal TotalPrice { get; set; }
	}
}
