using System.Collections.Generic;
using PertensaCo.Entities;
using PertensaCo.Web.Models;

namespace PertensaCo.Web.Areas.Logistics.Models
{
	public class SupplyCheckViewModel : ViewModel
	{
		public SupplyCheckViewModel()
		{
			Materials = new HashSet<Material>();
		}

		public IEnumerable<Material> Materials { get; set; }

		public override string PageTitle => "Supplies";
	}
}
