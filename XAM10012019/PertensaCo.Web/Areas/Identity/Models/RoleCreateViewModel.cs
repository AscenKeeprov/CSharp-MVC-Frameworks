using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.RoleConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
	public class RoleCreateViewModel : ViewModel
	{
		[Required(ErrorMessage = StringBlankErrorMessage)]
		[RegularExpression(RoleNamePattern,
			ErrorMessage = FormatMismatchErrorMessage)]
		[StringLength(maximumLength: RoleNameMaxLength,
			MinimumLength = RoleNameMinLength,
			ErrorMessage = StringMaxLengthErrorMessage)]
		public string Name { get; set; }

		[Display(Name = "Department")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string DepartmentName { get; set; }

		public IEnumerable<SelectListItem> Departments { get; set; }

		[Display(Name = "Display name")]
		[StringLength(maximumLength: RoleDisplayNameMaxLength,
			ErrorMessage = StringMaxLengthErrorMessage)]
		public string DisplayName { get; set; }

		public override string PageTitle => "Create role";
	}
}
