using System.ComponentModel.DataAnnotations;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Models
{
    public class SignInViewModel : ViewModel
    {
	[Required(ErrorMessage = StringBlankErrorMessage)]
	[Display(Name = "Alias / E-Mail")]
	public string Identity { get; set; }

	public override string PageTitle => "Sign in";

	[Required(ErrorMessage = StringBlankErrorMessage)]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	public bool RememberIdentity { get; set; }
    }
}
