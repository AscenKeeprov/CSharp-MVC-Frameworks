using System.Collections.Generic;
using System.Linq;
using Chushka.Data;
using Chushka.Models;
using Chushka.Models.Enumerations;
using Chushka.Services.Contracts;

namespace Chushka.Services
{
    public class ProductsService : IProductsService
    {
	private readonly ChushkaDbContext context;

	public ProductsService(ChushkaDbContext context)
	{
	    this.context = context;
	}

	public void AddProduct(string name, decimal price, string description, ProductType type)
	{
	    Product product = new Product()
	    {
		Name = name.Trim(),
		Description = description?.Trim(),
		Price = price,
		Type = type
	    };
	    context.Products.Add(product);
	    context.SaveChanges();
	}

	public void DeleteProduct(int id)
	{
	    Product product = context.Products.Find(id);
	    product.IsDeleted = true;
	    context.SaveChanges();
	}

	public bool Exists(string name)
	{
	    return context.Products.Any(p => p.Name == name);
	}

	public IEnumerable<Product> GetAllProducts()
	{
	    return context.Products
		.Where(p => !p.IsDeleted)
		.AsEnumerable();
	}

	public Product GetProductById(int id)
	{
	    return context.Products.Find(id);
	}

	public Product GetProductByName(string name)
	{
	    return context.Products.SingleOrDefault(p => p.Name == name);
	}

	public void UpdateProduct(int id, string name, decimal price, string description, ProductType type)
	{
	    Product product = context.Products.Find(id);
	    product.Name = name.Trim();
	    product.Price = price;
	    product.Description = description?.Trim();
	    product.Type = type;
	    context.SaveChanges();
	}
    }
}
