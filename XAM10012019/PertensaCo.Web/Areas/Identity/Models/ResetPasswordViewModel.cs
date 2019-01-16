using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
    public class ResetPasswordViewModel : ViewModel
    {
	[Email]
	[Display(Name = "E-mail address")]
	public string Email { get; set; }

	public override string PageTitle => "Reset password";

	[Password]
	public string Password { get; set; }

	[FromQuery(Name = "prt")]
	public string PasswordResetToken { get; set; }

	[Password]
	[Display(Name = "Confirm password")]
	[Compare(nameof(Password), ErrorMessage = PasswordsMismatchErrorMessage)]
	public string RepeatPassword { get; set; }
    }
}
