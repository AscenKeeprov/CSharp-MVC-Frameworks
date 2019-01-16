using Microsoft.AspNetCore.Identity;

namespace PertensaCo.Entities
{
    public class UserToken : IdentityUserToken<string>
    {
	public virtual User User { get; set; }

	public override string ToString()
	{
	    return $"{Name}: {Value ?? LoginProvider}";
	}
    }
}
