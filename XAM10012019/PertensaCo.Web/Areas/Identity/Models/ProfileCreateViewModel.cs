using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
	public class ProfileCreateViewModel : ViewModel
	{
		[Username]
		public string Alias { get; set; }

		[DataType(DataType.DateTime)]
		[Display(Name = "Active from")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public DateTimeOffset DateRegistered { get; set; }

		[Email]
		[Display(Name = "E-mail address")]
		public string EmailAddress { get; set; }

		[Display(Name = "Employee")]
		public string EmployeeId { get; set; }

		public IEnumerable<SelectListItem> Employees { get; set; }

		public override string PageTitle => "New profile";

		[Password]
		[Display(Name = "Initial password")]
		public string Password { get; set; }

		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }

		[Password]
		[Display(Name = "Confirm password")]
		[Compare(nameof(Password), ErrorMessage = PasswordsMismatchErrorMessage)]
		public string RepeatPassword { get; set; }
	}
}
