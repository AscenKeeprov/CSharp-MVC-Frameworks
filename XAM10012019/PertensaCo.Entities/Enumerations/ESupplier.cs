using System.ComponentModel;

namespace PertensaCo.Entities.Enumerations
{
	public enum ESupplier : byte
	{
		[DisplayName("Abundance Incorporated")]
		Abundant = 4,
		[DisplayName("Meagre Industries")]
		Meagre = 15,
		None = 0,
		[DisplayName("Satis Factories")]
		Sufficient = 9,
	}
}
