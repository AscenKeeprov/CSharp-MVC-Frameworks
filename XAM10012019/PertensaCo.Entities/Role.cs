using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using PertensaCo.Entities.Enumerations;

namespace PertensaCo.Entities
{
	public class Role : IdentityRole
	{
		public Role() : base() { }

		public Role(string roleName) : base(roleName) { }

		public EDepartment Department { get; set; }
		public string DisplayName { get; set; }

		public virtual ICollection<RoleClaim> Claims { get; set; } = new HashSet<RoleClaim>();
		public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
		public virtual ICollection<UserRole> Users { get; set; } = new HashSet<UserRole>();

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(DisplayName))
			{
				return DisplayName;
			}
			else return Name;
		}
	}
}
