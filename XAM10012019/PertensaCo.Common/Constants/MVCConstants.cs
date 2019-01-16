namespace PertensaCo.Common.Constants
{
	public static class MVCConstants
	{
		public const string ConfirmEmailTemplatePath = "/Templates/ConfirmEmailTemplate.html";
		public const string ControllerActionRouteTemplate = "[controller]/[action]";
		public const string EmployeesAuthorizationPolicyName = "EmployeesOnly";
		public const string ErrorViewRoute = "/Error";
		public const string ForbiddenViewRoute = "/Forbidden";
		public const string HumanResourcesAuthorizationPolicyName = "HROnly";
		public const string InformationTechnologiesAuthorizationPolicyName = "ITOnly";
		public const string LoginViewRoute = "/SignIn";
		public const string LogisticsAuthorizationPolicyName = "LnPOnly";
		public const string LogoutViewRoute = "/SignOut";
		public const int ModelMaxValidationErrors = 16;
		public const string PersonnelDataRoot = "/Areas/Personnel/Dossiers";
		public const string ResetPasswordTemplatePath = "/Templates/ResetPasswordTemplate.html";
		public const string RoleAdministratorsAuthorizationPolicyName = "RoleAdminsOnly";
		public const string ScientistsAuthorizationPolicyName = "RnDOnly";
		public const string UserAlreadyInRoleErrorCode = "UserAlreadyInRole";
		public const int ViewMaxStatusMessageLines = 8;
		public const string WebUsersAuthorizationPolicyName = "WebUsersOnly";
	}
}
