namespace PertensaCo.Common.Constants
{
	public static class UserConstants
	{
		public const int AddressMaxLength = 128;
		public const int CompanyNameMaxLength = 64;
		public const bool EmailIsUnicode = false;
		public const bool EmailIsUnique = true;
		public const int EmailMaxLength = 64;
		public const string EmailPattern = @"^[^@\s.]+(?:\.[^@\s.]+)*\@[^@\s.]+(?:\.[^@\s.]+)+$";
		public const bool EmailRequireConfirmed = false;
		public const bool LockoutEnabledWhenNew = false;
		public const int LoginProviderKeyMaxLength = 224;
		public const int LoginProviderMaxLength = 224;
		public const int LoginProviderNameMaxLength = 64;
		public const bool PasswordHasDigit = true;
		public const bool PasswordHasLowercase = true;
		public const int PasswordHashMaxLength = 96;
		public const bool PasswordHasSymbol = true;
		public const bool PasswordHasUppercase = true;
		public const int PasswordMinLength = 8;
		public const string PasswordPattern = @"^(?=\S*[a-z])(?=\S*[A-Z])(?=\S*\d)(?=\S*[^a-zA-Z\d\s])[^\s%&?*'""]+$";
		public const int PersonalIdentificationNumberMaxLength = 18;
		public const int PersonalIdentificationNumberMinLength = 6;
		public const string PersonalIdentificationNumberPattern = @"^[()\w.-]{6,18}$";
		public const int PersonalNameMaxLength = 32;
		public const string PersonalNamePattern = @"^([\p{Lu}\p{L}][\p{L}\p{Lu} .-]*)$";
		public const int PhoneNumberMaxLength = 32;
		public const bool PhoneNumberRequireConfirmed = false;
		public const int SecurityStampMaxLength = 32;
		public const int TokenNameMaxLength = 128;
		public const string UsernameAllowedCharacters =
			"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@$!#+~-._";
		public const bool UsernameIsUnicode = false;
		public const bool UsernameIsUnique = true;
		public const int UsernameMaxLength = 64;
		public const int UsernameMinLength = 4;
		public const string UsernamePattern = @"^(?=\S*[a-z])[\w.@$!#+~-]+$";
	}
}
