using System.Collections.Generic;
using Chushka.Models;
using Chushka.Models.Enumerations;

namespace Chushka.Services.Contracts
{
    public interface IProductsService
    {
	IEnumerable<Product> GetAllProducts();
	bool Exists(string name);
	void AddProduct(string name, decimal price, string description, ProductType type);
	Product GetProductById(int id);
	Product GetProductByName(string name);
	void UpdateProduct(int id, string name, decimal price, string description, ProductType type);
	void DeleteProduct(int id);
    }
}
