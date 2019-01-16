using PertensaCo.Entities.Enumerations;

namespace PertensaCo.Entities
{
	public class Material : Entity<int>
	{
		public Material(EElement element, EForm form, double quantityInKg)
		{
			Element = element;
			Form = form;
			PricePerKgInEur = ((int)element * 1.62M) + ((int)form * 3.14M);
			QuantityInKg = quantityInKg;
		}

		public EElement Element { get; set; }
		public EForm Form { get; set; }
		public decimal PricePerKgInEur { get; set; }
		public double QuantityInKg { get; set; }

		public override string ToString()
		{
			return $"[{Element.ToString()}] {Form}";
		}
	}
}
