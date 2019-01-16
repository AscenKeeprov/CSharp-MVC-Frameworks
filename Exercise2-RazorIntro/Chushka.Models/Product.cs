using System.Collections.Generic;
using Chushka.Models.Enumerations;

namespace Chushka.Models
{
    public class Product
    {
	public string Description { get; set; }
	public int Id { get; set; }
	public bool IsDeleted { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public ProductType Type { get; set; }

	public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
