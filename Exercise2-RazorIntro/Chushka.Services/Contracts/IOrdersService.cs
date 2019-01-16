using System.Collections.Generic;
using Chushka.Models;

namespace Chushka.Services.Contracts
{
    public interface IOrdersService
    {
	void CreateOrder(int productId, string clientName);
	IEnumerable<Order> GetAllOrders();
    }
}
