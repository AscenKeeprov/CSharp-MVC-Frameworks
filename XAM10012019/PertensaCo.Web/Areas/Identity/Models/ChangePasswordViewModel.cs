using System.ComponentModel.DataAnnotations;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
    public class ChangePasswordViewModel : ViewModel
    {
	[Password]
	[Display(Name = "New password")]
	public string NewPassword { get; set; }

	[Password]
	[Display(Name = "Current password")]
	public string OldPassword { get; set; }

	public override string PageTitle => "Change password";

	[Password]
	[Display(Name = "Confirm new password")]
	[Compare(nameof(NewPassword), ErrorMessage = PasswordsMismatchErrorMessage)]
	public string RepeatNewPassword { get; set; }
    }
}
