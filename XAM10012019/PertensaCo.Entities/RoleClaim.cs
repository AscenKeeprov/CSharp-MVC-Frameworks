using Microsoft.AspNetCore.Identity;

namespace PertensaCo.Entities
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
	public virtual Role Role { get; set; }

	public override string ToString()
	{
	    if (ClaimType != null && ClaimValue != null)
	    {
		return $"{ClaimType}: {ClaimValue}";
	    }
	    return base.ToString();
	}
    }
}
