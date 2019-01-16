using Chushka.Services.Contracts;
using Chushka.Web.Controllers;
using Chushka.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Chushka.Common.Constants;

namespace Chushka.App.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
	private readonly IProductsService productsService;

	public ProductsController(IProductsService productsService)
	{
	    this.productsService = productsService;
	}

	[HttpGet]
	public IActionResult Create()
	{
	    var model = new ProductViewModel();
	    return View(model);
	}

	[HttpPost]
	public IActionResult Create(ProductViewModel model)
	{
	    if (!ModelState.IsValid)
	    {
		ViewData[ErrorKey] = InvalidModelSateError;
		return View(model);
	    }
	    if (productsService.Exists(model.Name))
	    {
		ViewData[ErrorKey] = $"Product '{model.Name}' is already in stock";
		return View(model);
	    }
	    productsService.AddProduct(model.Name, model.Price, model.Description, model.Type);
	    return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	public IActionResult Delete(int id)
	{
	    var product = productsService.GetProductById(id);
	    var model = new ProductViewModel(product);
	    return View(model);
	}

	[HttpPost]
	public IActionResult Delete(ProductViewModel model)
	{
	    productsService.DeleteProduct(model.Id);
	    return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	public IActionResult Details(int id)
	{
	    var product = productsService.GetProductById(id);
	    var model = new ProductViewModel(product);
	    return View(model);
	}

	[HttpGet]
	public IActionResult Edit(int id)
	{
	    var product = productsService.GetProductById(id);
	    var model = new ProductViewModel(product);
	    return View(model);
	}

	[HttpPost]
	public IActionResult Edit(ProductViewModel model)
	{
	    if (!ModelState.IsValid)
	    {
		ViewData[ErrorKey] = InvalidModelSateError;
		return View(model);
	    }
	    var existingProduct = productsService.GetProductByName(model.Name);
	    if (existingProduct != null && existingProduct.Id != model.Id)
	    {
		ViewData[ErrorKey] = $"Product '{model.Name}' is already in stock";
		return View(model);
	    }
	    productsService.UpdateProduct(model.Id, model.Name, model.Price, model.Description, model.Type);
	    return RedirectToAction("Index", "Home");
	}
    }
}
