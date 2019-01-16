using System.ComponentModel.DataAnnotations;
using PertensaCo.Web.Attributes;

namespace PertensaCo.Web.Areas.Identity.Models
{
    public class ClientProfileViewModel : ProfileViewModel
    {
	[Display(Name = "Company")]
	public string CompanyName { get; set; }

	[PersonalName]
	[Display(Name = "First name")]
	public string FirstName { get; set; }

	[PersonalName]
	[Display(Name = "Last name")]
	public string LastName { get; set; }

	public override string PageTitle => base.PageTitle;

	[Display(Name = "Shipping address")]
	public string ShippingAddress { get; set; }
    }
}
