namespace PertensaCo.Common.Constants
{
	public static class GlobalConstants
	{
		public const string AliasKey = "Alias";
		public const string CompanyName = "Pertensa Co.";
		public const string DateDisplayFormat = "d MMMM yyyy";
		public const string DateInPastErrorMessage = "{0} cannot be in the past!";
		public const string DateTimeFormat = "dd-MMM-yyyy HH:mm:ss";
		public const string DepartmentKey = "Department";
		public const string EmployeeIdKey = "EmployeeId";
		public const string ErrorKey = "Error";
		public const int FileNameMaxLength = 64;
		public const string FormatMismatchErrorMessage = "{0} is not in the correct format!";
		public const int GuidStringLength = 36;
		public const string InfoKey = "Info";
		public const string InvalidObjectErrorMessage = "{0} is not valid!";
		public const string LogFilePath = "/Logs/{0}_.log";
		public const string LogEntryTemplate = "[{Timestamp:{0}}] [{Level:u}] {Message}{NewLine}{Exception}";
		public const string LockoutErrorMessage = "{0} is locked out until {1}.";
		public const string ObjectExistsErrorMessage = "{0} already exists.";
		public const string ObjectNotFoundErrorMessage = "{0} could not be found.";
		public const string ObjectsShouldNotMatchErrorMessage = "{0} and {1} should not be the same!";
		public const string PasswordsMismatchErrorMessage = "Passwords do not match each other.";
		public const string ProfileIdKey = "ProfileId";
		public const string RangeErrorMessage = "{0} must be between {1} and {2}!";
		public const string RoleKey = "Role";
		public const string SelectionNullErrorMessage = "You must select {0}.";
		public const string StringBlankErrorMessage = "{0} cannot be blank!";
		public const string StringLengthErrorMessage = "{0} must be between {2} and {1} characters long!";
		public const string StringMaxLengthErrorMessage = "{0} length cannot exceed {1} characters!";
		public const string StringMinLengthErrorMessage = "{0} cannot be shorter than {1} characters!";
		public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss zzz";
		public const string TitleKey = "Title";
		public const string TraceIdKey = "TraceId";
		public const string TypeKey = "Type";
		public const string UserIdKey = "UserId";
	}
}
