namespace PertensaCo.Entities
{
	public class Alloy : Entity<int>
	{
		public Alloy() { }

		public Alloy(string composition)
		{
			Composition = composition;
		}

		public Alloy(string composition, decimal researchCost) : this(composition)
		{
			ResearchCost = researchCost;
		}

		public string Composition { get; set; }
		public decimal ResearchCost { get; set; }

		public override string ToString()
		{
			return Composition;
		}
	}
}
