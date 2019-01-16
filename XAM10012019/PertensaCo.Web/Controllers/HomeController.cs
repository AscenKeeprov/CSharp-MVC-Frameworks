using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Models;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet(nameof(About))]
		public IActionResult About()
		{
			return View();
		}

		[HttpGet(nameof(Contacts))]
		public IActionResult Contacts()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpGet(nameof(Error))]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			var model = new ErrorViewModel()
			{
				ErrorId = HttpContext.GenerateErrorId()
			};
			return View(model);
		}

		[AllowAnonymous]
		[HttpGet(nameof(Forbidden))]
		public IActionResult Forbidden()
		{
			return View();
		}

		[HttpGet("/")]
		public IActionResult Index()
		{
			HttpContext.Session?.SetString(TraceIdKey, HttpContext.TraceIdentifier);
			return View();
		}

		[HttpGet(nameof(Privacy))]
		public IActionResult Privacy()
		{
			return View();
		}
	}
}
