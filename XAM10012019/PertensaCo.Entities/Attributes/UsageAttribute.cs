using System;

namespace PertensaCo.Entities.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class UsageAttribute : Attribute
	{
		public UsageAttribute(bool Additive = false, bool Base = false, bool Plate = false)
		{
			this.IsUsedAsAdditive = Additive;
			this.IsUsedAsBase = Base;
			this.IsUsedAsPlate = Plate;
		}

		public bool IsUsedAsBase { get; }
		public bool IsUsedAsAdditive { get; }
		public bool IsUsedAsPlate { get; }
	}
}
