namespace PertensaCo.Common.Constants
{
	public static class DBConstants
	{
		public const string AliasColumnName = "Alias";
		public const string DateFunc = "CONVERT(DATE,GETDATE())";
		public const string DateTimeUtcFunc = "SYSDATETIMEOFFSET()";
		public const string DateTimeUtcNoTicksType = "DATETIMEOFFSET(0)";
		public const string DateType = "DATE";
		public const string DecimalPrecision16Scale2Type = "DECIMAL(16,2)";
		public const string DecimalPrecision9Scale2Type = "DECIMAL(9,2)";
		public const string MaterialsSeedFilePath = "/Seeds/materials.json";
		public const string NormalizedAliasColumnName = "NormalizedAlias";
		public const string NumericPrecision9Scale3Type = "NUMERIC(9,3)";
		public const string ProfilesSeedFilePath = "/Seeds/profiles.json";
		public const string RolesSeedFilePath = "/Seeds/roles.json";
		public const string TinyIntType = "TINYINT";
		public const string TokenNameColumnName = "TokenName";
		public const string TokenValueColumnName = "TokenValue";
	}
}
