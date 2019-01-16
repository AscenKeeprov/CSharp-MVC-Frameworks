using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FDMC.Web.Data;
using FDMC.Web.Entities;
using FDMC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FDMC.Web.Controllers
{
    public class CatsController : Controller
    {
	private readonly FDMCDbContext dbContext;

	public CatsController(FDMCDbContext dbContext)
	{
	    this.dbContext = dbContext;
	}

	[HttpGet("/cats/{id}")]
	public async Task<IActionResult> Details(int? id)
	{
	    if (id == null) return NotFound();
	    var cat = await dbContext.Cats.FindAsync(id);
	    if (cat == null) return NotFound();
	    return View(cat);
	}

	[HttpGet("/cats/add")]
	public IActionResult Add()
	{
	    return View();
	}

	[HttpPost("/cats/add")]
	public async Task<IActionResult> Add(Cat cat)
	{
	    if (!ModelState.IsValid) return View(cat);
	    try
	    {
		dbContext.Cats.Add(cat);
		await dbContext.SaveChangesAsync();
	    }
	    catch (DbUpdateException exception)
	    {
		var innerException = exception.InnerException;
		if (innerException is SqlException && ((SqlException)innerException).Number == 2601)
		{
		    var model = new ErrorViewModel()
		    {
			Message = $"Cat '{cat.Name}', {cat.Age}-years-old {cat.Breed} has already been added!"
		    };
		    return View("Error", model);
		}
	    }
	    return Redirect("/");
	}
    }
}
