using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PertensaCo.Web.Models;

namespace PertensaCo.Web.Areas.Innovation.Models
{
	public class ResearchViewModel : ViewModel
	{
		public IEnumerable<SelectListItem> AdditionalMaterials { get; set; }

		[Display(Name = "Additives")]
		public IEnumerable<string> AdditiveNames { get; set; }

		[Display(Name = "Base")]
		public string BaseName { get; set; }

		public IEnumerable<SelectListItem> BaseMaterials { get; set; }

		public override string PageTitle => "Conduct experiment";
	}
}
