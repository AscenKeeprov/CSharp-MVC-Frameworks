using System.Globalization;
using System.Linq;
using Chushka.Services.Contracts;
using Chushka.Web.Controllers;
using Chushka.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Chushka.Common.Constants;

namespace Chushka.App.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
	private readonly IOrdersService ordersService;

	public OrdersController(IOrdersService ordersService)
	{
	    this.ordersService = ordersService;
	}

	[HttpGet]
	public IActionResult All()
	{
	    var orders = ordersService.GetAllOrders()
		.Select(order => new OrderViewModel()
		{
		    OrderId = order.Id,
		    CustomerName = order.Client.FullName ?? order.Client.UserName,
		    ProductName = order.Product.Name,
		    OrderDate = order.OrderedOn.ToString(DateTimeFormat, CultureInfo.InvariantCulture)
		}).ToArray();
	    return View(orders);
	}

	[HttpGet]
	public IActionResult Create(int productId)
	{
	    string clientName = User.Identity.Name;
	    ordersService.CreateOrder(productId, clientName);
	    return RedirectToAction("Index", "Home");
	}
    }
}
