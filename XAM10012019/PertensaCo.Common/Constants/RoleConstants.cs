namespace PertensaCo.Common.Constants
{
    public static class RoleConstants
    {
	public const int RoleDisplayNameMaxLength = 64;
	public const int RoleNameMaxLength = 32;
	public const int RoleNameMinLength = 4;
	public const string RoleNamePattern = @"^[A-Z][\w]{3,}$";
    }
}
