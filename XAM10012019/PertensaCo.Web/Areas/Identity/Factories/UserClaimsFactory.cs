using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PertensaCo.Entities;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Areas.Identity.Factories
{
	public class UserClaimsFactory : UserClaimsPrincipalFactory<User, Role>
	{
		public UserClaimsFactory(UserManager<User> profileService, RoleManager<Role> roleService,
			IOptions<IdentityOptions> options) : base(profileService, roleService, options) { }

		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User profile)
		{
			ClaimsIdentity claims = await base.GenerateClaimsAsync(profile);
			if (!claims.HasClaim(c => c.Type == ProfileIdKey))
			{
				var profileIdClaim = new Claim(ProfileIdKey, profile.Id);
				claims.AddClaim(profileIdClaim);
			}
			if (!claims.HasClaim(c => c.Type == AliasKey))
			{
				var aliasClaim = new Claim(AliasKey, profile.UserName);
				claims.AddClaim(aliasClaim);
			}
			if (!claims.HasClaim(c => c.Type == TypeKey))
			{
				string typeValue = nameof(User);
				if (profile.Client != null) typeValue = nameof(Client);
				if (profile.Employee != null) typeValue = nameof(Employee);
				var typeClaim = new Claim(TypeKey, typeValue);
				claims.AddClaim(typeClaim);
			}
			if (profile.Employee != null)
			{
				if (!claims.HasClaim(c => c.Type == DepartmentKey))
				{
					string departmentValue = profile.Employee.Department.ToString();
					var departmentClaim = new Claim(DepartmentKey, departmentValue);
					claims.AddClaim(departmentClaim);
				}
				if (!claims.HasClaim(c => c.Type == EmployeeIdKey))
				{
					var employeeIdValue = profile.Employee.Id.ToString();
					var employeeIdClaim = new Claim(EmployeeIdKey, employeeIdValue);
					claims.AddClaim(employeeIdClaim);
				}
			}
			return claims;
		}
	}
}
