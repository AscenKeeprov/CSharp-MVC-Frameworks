using System.ComponentModel.DataAnnotations;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
	public class RegisterViewModel : ViewModel
	{
		[Username]
		public string Alias { get; set; }

		[Email]
		[Display(Name = "E-mail address")]
		public string Email { get; set; }

		public override string PageTitle => "Registration";

		[Password]
		public string Password { get; set; }

		[Password]
		[Display(Name = "Confirm password")]
		[Compare(nameof(Password), ErrorMessage = PasswordsMismatchErrorMessage)]
		public string RepeatPassword { get; set; }
	}
}
