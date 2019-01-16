using System;
using System.Linq;
using System.Security.Claims;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		/// <summary>
		/// Attempts to read the principal's identifier from the claims store.
		/// <para>Returns null if no match is found.</para>
		/// </summary>
		public static string GetId(this ClaimsPrincipal principal)
		{
			string identifier = principal.FindFirstValue(ProfileIdKey);
			if (string.IsNullOrWhiteSpace(identifier))
			{
				identifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			}
			return identifier;
		}

		public static bool HasDepartment(this ClaimsPrincipal principal, string value)
		{
			if (!principal.Claims.Any()) return false;
			string departmentValue = principal.FindFirstValue(DepartmentKey);
			if (string.IsNullOrWhiteSpace(departmentValue)) return false;
			return departmentValue.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool HasRole(this ClaimsPrincipal principal, string value)
		{
			if (!principal.Claims.Any()) return false;
			string[] roleValues = principal.FindAll(RoleKey)
				.Select(c => c.Value).ToArray();
			if (!roleValues.Any()) return false;
			return roleValues.Any(rv => rv.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		}

		public static bool HasType(this ClaimsPrincipal principal, string value)
		{
			if (!principal.Claims.Any()) return false;
			string typeValue = principal.FindFirstValue(TypeKey);
			if (string.IsNullOrWhiteSpace(typeValue)) return false;
			return typeValue.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
