using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PertensaCo.Entities
{
    public class User : IdentityUser
    {
	public User() : base() { }

	public User(string userName) : base(userName) { }

	public DateTimeOffset? DateDeleted { get; set; }
	public DateTimeOffset? DateLastLogin { get; set; }
	public DateTimeOffset DateRegistered { get; set; }

	public virtual Client Client { get; set; }
	public virtual Employee Employee { get; set; }

	public virtual ICollection<UserClaim> Claims { get; set; } = new HashSet<UserClaim>();
	public virtual ICollection<UserLogin> Logins { get; set; } = new HashSet<UserLogin>();
	public virtual ICollection<UserRole> Roles { get; set; } = new HashSet<UserRole>();
	public virtual ICollection<UserToken> Tokens { get; set; } = new HashSet<UserToken>();

	public override string ToString()
	{
	    return UserName;
	}
    }
}
