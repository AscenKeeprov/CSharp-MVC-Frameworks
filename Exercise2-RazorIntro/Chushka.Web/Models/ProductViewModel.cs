using System;
using System.ComponentModel.DataAnnotations;
using Chushka.Models;
using Chushka.Models.Enumerations;

namespace Chushka.Web.Models
{
    public class ProductViewModel
    {
	public ProductViewModel() { }

	public ProductViewModel(Product product)
	{
	    Id = product.Id;
	    Name = product.Name;
	    Type = product.Type;
	    Price = product.Price;
	    Description = product.Description;
	}

	public string Description { get; set; }

	public int Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Required]
	[DataType(DataType.Currency)]
	public decimal Price { get; set; }

	[Required]
	public ProductType Type { get; set; }

	public Array ProductTypes => Enum.GetValues(typeof(ProductType));
    }
}
