using System.Collections.Generic;
using Chushka.Models.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace Chushka.Models
{
    public class User : IdentityUser
    {
	public string FullName { get; set; }
	public UserRole Role { get; set; }

	public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

	public override string ToString()
	{
	    return FullName ?? UserName;
	}
    }
}
