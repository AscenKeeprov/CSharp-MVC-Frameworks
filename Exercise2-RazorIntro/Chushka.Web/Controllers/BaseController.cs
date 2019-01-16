using System.Diagnostics;
using Chushka.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chushka.Web.Controllers
{
    public class BaseController : Controller
    {
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
	    var model = new ErrorViewModel()
	    {
		RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
	    };
	    return View(model);
	}
    }
}