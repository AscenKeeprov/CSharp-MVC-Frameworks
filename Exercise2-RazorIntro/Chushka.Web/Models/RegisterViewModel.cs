using System.ComponentModel.DataAnnotations;

namespace Chushka.Web.Models
{
    public class RegisterViewModel
    {
	[Required]
	public string Username { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	[Required]
	[Compare(nameof(Password))]
	[DataType(DataType.Password)]
	[Display(Name = "Confirm Password")]
	public string ConfirmPassword { get; set; }

	[Display(Name = "Full Name")]
	public string FullName { get; set; }

	[DataType(DataType.EmailAddress)]
	[Display(Name = "E-mail Address")]
	public string Email { get; set; }
    }
}
