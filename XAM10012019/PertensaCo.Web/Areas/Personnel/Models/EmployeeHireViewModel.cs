using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.EmployeeConstants;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Web.Areas.Personnel.Models
{
	public class EmployeeHireViewModel : ViewModel
	{
		public EmployeeHireViewModel()
		{
			Departments = new List<SelectListItem>();
			Managers = new List<SelectListItem>();
			Profiles = new List<SelectListItem>();
			Roles = new List<SelectListItem>();
		}

		[DataType(DataType.Date)]
		[Display(Name = "Contract start")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public DateTime DateHired { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Contract end")]
		public DateTime? DateRelieved { get; set; }

		[Display(Name = "Department")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string DepartmentName { get; set; }

		public IEnumerable<SelectListItem> Departments { get; set; }

		[PersonalName]
		[Display(Name = "First name")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string FirstName { get; set; }

		[Display(Name = "Home address")]
		[StringLength(AddressMaxLength)]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string HomeAddress { get; set; }

		[Display(Name = "Temporary contractor")]
		public bool IsTemporary { get; set; }

		[PersonalName]
		[Display(Name = "Last name")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string LastName { get; set; }

		[Display(Name = "Manager")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string ManagerId { get; set; }

		public IEnumerable<SelectListItem> Managers { get; set; }

		[PersonalName]
		[Display(Name = "Middle name")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string MiddleName { get; set; }

		[Display(Name = "Salary")]
		[DataType(DataType.Currency)]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		[Range(typeof(decimal), minimum: SalaryMinTextValue, maximum: SalaryMaxTextValue,
			ErrorMessage = RangeErrorMessage)]
		public decimal MonthlySalaryInEUR { get; set; }

		public override string PageTitle => "New employee";

		[Display(Name = "National ID")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		[RegularExpression(PersonalIdentificationNumberPattern,
			ErrorMessage = FormatMismatchErrorMessage)]
		public string PIN { get; set; }

		[Display(Name = "Profile")]
		public string ProfileId { get; set; }

		[DataType(DataType.Upload)]
		[Display(Name = "Profile picture")]
		public IFormFile Portrait { get; set; }

		public IEnumerable<SelectListItem> Profiles { get; set; }

		[Display(Name = "Position")]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string RoleName { get; set; }

		public IEnumerable<SelectListItem> Roles { get; set; }

		[Display(Name = "Office location")]
		[StringLength(AddressMaxLength)]
		[Required(ErrorMessage = StringBlankErrorMessage)]
		public string WorkAddress { get; set; }
	}
}
