using System.ComponentModel.DataAnnotations;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;

namespace PertensaCo.Web.Areas.Identity.Models
{
    public class ForgotPasswordViewModel : ViewModel
    {
	[Email]
	[Display(Name = "E-mail address")]
	public string Email { get; set; }

	public override string PageTitle => "Forgotten password";

	public bool PasswordSent { get; set; }
    }
}
