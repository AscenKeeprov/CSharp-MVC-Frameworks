using Microsoft.AspNetCore.Identity;

namespace PertensaCo.Entities
{
    public class UserLogin : IdentityUserLogin<string>
    {
	public virtual User User { get; set; }

	public override string ToString()
	{
	    return ProviderDisplayName ?? LoginProvider;
	}
    }
}
