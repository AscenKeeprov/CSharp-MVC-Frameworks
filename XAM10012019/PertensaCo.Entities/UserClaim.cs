using Microsoft.AspNetCore.Identity;

namespace PertensaCo.Entities
{
    public class UserClaim : IdentityUserClaim<string>
    {
	public virtual User User { get; set; }

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
