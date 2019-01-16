using System.ComponentModel;
using PertensaCo.Entities.Attributes;

namespace PertensaCo.Entities.Enumerations
{
	public enum EElement : byte
	{
		[Usage(Additive: true, Plate: true)]
		[DisplayName("Silver")]
		Ag = 47,

		[Usage(Additive: true, Base: true)]
		[DisplayName("Aluminium")]
		Al = 13,

		[Usage(Plate: true)]
		[DisplayName("Boron")]
		B = 5,

		[Usage(Plate: true)]
		[DisplayName("Cadmium")]
		Cd = 48,

		[Usage(Additive: true, Plate: true)]
		[DisplayName("Cobalt")]
		Co = 27,

		[Usage(Additive: true, Plate: true)]
		[DisplayName("Chromium")]
		Cr = 24,

		[Usage(Additive: true, Plate: true)]
		[DisplayName("Copper")]
		Cu = 29,

		[Usage(Additive: true, Base: true)]
		[DisplayName("Iron")]
		Fe = 26,

		[Usage(Additive: true, Base: true)]
		[DisplayName("Hafnium")]
		Hf = 72,

		[Usage(Plate: true)]
		[DisplayName("Indium")]
		In = 49,

		[Usage(Additive: true)]
		[DisplayName("Manganese")]
		Mn = 25,

		[Usage(Additive: true)]
		[DisplayName("Molybdenum")]
		Mo = 42,

		[Usage(Additive: true, Base: true)]
		[DisplayName("Niobium")]
		Nb = 41,

		[Usage(Additive: true, Plate: true)]
		[DisplayName("Nickel")]
		Ni = 28,

		[Usage(Additive: true)]
		[DisplayName("Palladium")]
		Pd = 46,

		[Usage(Plate: true)]
		[DisplayName("Ruthenium")]
		Ru = 44,

		[Usage(Additive: true, Plate: true)]
		[DisplayName("Tin")]
		Sn = 50,

		[Usage(Additive: true)]
		[DisplayName("Tantalium")]
		Ta = 73,

		[Usage(Additive: true)]
		[DisplayName("Tellurium")]
		Te = 52,

		[Usage(Additive: true)]
		[DisplayName("Titanium")]
		Ti = 22,

		[Usage(Additive: true)]
		[DisplayName("Vanadium")]
		V = 23,

		[Usage(Additive: true)]
		[DisplayName("Wolfram")]
		W = 74,

		[Usage(Additive: true, Plate: true)]
		[DisplayName("Zinc")]
		Zn = 30,
	}
}
