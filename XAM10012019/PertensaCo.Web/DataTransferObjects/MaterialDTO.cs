using PertensaCo.Entities.Enumerations;

namespace PertensaCo.Web.DataTransferObjects
{
	public class MaterialDTO
	{
		public string Code { get; set; }
		public EElement Element { get; set; }
		public EForm Form { get; set; }
		public double Quantity { get; set; }
		public string Type { get; set; }
	}
}
