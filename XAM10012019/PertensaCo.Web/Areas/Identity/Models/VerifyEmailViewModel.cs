using System;
using Microsoft.AspNetCore.Mvc;
using PertensaCo.Web.Models;

namespace PertensaCo.Web.Areas.Identity.Models
{
	public class VerifyEmailViewModel : ViewModel
	{
		[FromQuery(Name = "ect")]
		public string EmailConfirmationToken { get; set; }

		public Exception Exception { get; set; }
		public string FailureReason { get; set; }
		public override string PageTitle => "Verify e-mail";
		public string ProfileName { get; set; }

		[FromQuery(Name = "pid")]
		public string ProfileId { get; set; }
	}
}
