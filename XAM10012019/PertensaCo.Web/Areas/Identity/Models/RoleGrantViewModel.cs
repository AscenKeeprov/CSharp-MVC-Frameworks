using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
	public class RoleGrantViewModel : ViewModel
	{
		public RoleGrantViewModel()
		{
			AvailableProfiles = new List<SelectListItem>();
			Roles = new List<SelectListItem>();
			SelectedProfiles = new List<string>();
		}

		public IEnumerable<SelectListItem> AvailableProfiles { get; set; }

		public override string PageTitle => "Assign roles";

		[Display(Name = "Role")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string RoleName { get; set; }

		public IEnumerable<SelectListItem> Roles { get; set; }

		[Display(Name = "Profiles")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public IEnumerable<string> SelectedProfiles { get; set; }
	}
}
