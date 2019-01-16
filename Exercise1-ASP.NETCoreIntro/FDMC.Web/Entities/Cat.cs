using System.ComponentModel.DataAnnotations;

namespace FDMC.Web.Entities
{
    public class Cat
    {
	[Key]
	public int Id { get; set; }

	[Required]
	[RegularExpression(@"[a-zA-Z\- ]+")]
	public string Name { get; set; }

	[Required]
	[Range(0, 28)]
	public int Age { get; set; }

	[Required]
	[RegularExpression(@"[a-zA-Z\- ]+")]
	public string Breed { get; set; }

	[Required]
	[DataType(DataType.Url)]
	public string ImageUrl { get; set; }
    }
}
