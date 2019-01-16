using System.ComponentModel.DataAnnotations.Schema;

namespace PertensaCo.Entities
{
	public class Client : Entity<int>
	{
		public string CompanyName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ShippingAddress { get; set; }

		[ForeignKey(nameof(Profile))]
		public string ProfileId { get; set; }
		public virtual User Profile { get; set; }

		public override string ToString()
		{
			string displayName = FirstName;
			if (!string.IsNullOrWhiteSpace(LastName))
			{
				displayName += $" {LastName}";
			}
			if (string.IsNullOrWhiteSpace(displayName))
			{
				displayName = Profile.UserName;
			}
			return displayName;
		}
	}
}
