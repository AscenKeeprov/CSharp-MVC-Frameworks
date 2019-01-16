using System.Linq;
using Chushka.Services.Contracts;
using Chushka.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chushka.Web.Controllers
{
    public class HomeController : BaseController
    {
	private readonly IProductsService productsService;

	public HomeController(IProductsService productsService)
	{
	    this.productsService = productsService;
	}

	[HttpGet]
	public IActionResult Index()
	{
	    var products = productsService
		.GetAllProducts()
		.Select(product => new ProductViewModel()
		{
		    Id = product.Id,
		    Name = product.Name,
		    Description = product.Description?.Length > 53
		    ? $"{string.Join("", product.Description.Take(50))}..."
		    : product.Description,
		    Price = product.Price
		}).ToArray();
	    return View(products);
	}
    }
}
