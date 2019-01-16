using System.Linq;
using FDMC.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace FDMC.Web.Controllers
{
    public class HomeController : Controller
    {
	private readonly FDMCDbContext dbContext;

	public HomeController(FDMCDbContext dbContext)
	{
	    this.dbContext = dbContext;
	}

	[HttpGet("/")]
	public IActionResult Index()
	{
	    return View(dbContext.Cats.AsEnumerable());
	}
    }
}
