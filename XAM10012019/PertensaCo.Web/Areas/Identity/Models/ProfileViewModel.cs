using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PertensaCo.Web.Attributes;
using PertensaCo.Web.Models;

namespace PertensaCo.Web.Areas.Identity.Models
{
	public class ProfileViewModel : ViewModel
	{
		[Username]
		public string Alias { get; set; }

		[Display(Name = "Registered on:")]
		public string DateRegistered { get; set; }

		[Email]
		[Display(Name = "E-mail address")]
		public string EmailAddress { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[FromQuery(Name = "pid")]
		public string Id { get; set; }

		public override string PageTitle => "Manage profile";

		[DataType(DataType.PhoneNumber)]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }
	}
}
