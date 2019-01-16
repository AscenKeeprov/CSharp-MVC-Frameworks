using System;
using System.Collections.Generic;
using System.Linq;
using Chushka.Data;
using Chushka.Models;
using Chushka.Services.Contracts;

namespace Chushka.Services
{
    public class OrdersService : IOrdersService
    {
	private readonly ChushkaDbContext context;

	public OrdersService(ChushkaDbContext context)
	{
	    this.context = context;
	}

	public void CreateOrder(int productId, string clientName)
	{
	    Product product = context.Products.Find(productId);
	    User client = context.Users
		.SingleOrDefault(u => u.UserName == clientName);
	    Order order = new Order()
	    {
		Product = product,
		Client = client,
		OrderedOn = DateTime.UtcNow
	    };
	    context.Orders.Add(order);
	    context.SaveChanges();
	}

	public IEnumerable<Order> GetAllOrders()
	{
	    return context.Orders.AsEnumerable();
	}
    }
}
