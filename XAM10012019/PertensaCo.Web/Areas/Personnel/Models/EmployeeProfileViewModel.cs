using System.ComponentModel.DataAnnotations;
using PertensaCo.Web.Areas.Identity.Models;
using PertensaCo.Web.Attributes;

namespace PertensaCo.Web.Areas.Personnel.Models
{
	public class EmployeeProfileViewModel : ProfileViewModel
	{
		[Display(Name = "Contract start date:")]
		public string DateHired { get; set; }

		[Display(Name = "Contract end date:")]
		public string DateRelieved { get; set; }

		public string Department { get; set; }

		[PersonalName]
		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[PersonalName]
		[Display(Name = "Last name")]
		public string LastName { get; set; }

		public string Manager { get; set; }

		[PersonalName]
		[Display(Name = "Middle name")]
		public string MiddleName { get; set; }

		public override string PageTitle => "Employee profile";

		[Display(Name = "Home address")]
		public string HomeAddress { get; set; }

		public decimal Salary { get; set; }

		[Display(Name = "Office location")]
		public string WorkAddress { get; set; }
	}
}
