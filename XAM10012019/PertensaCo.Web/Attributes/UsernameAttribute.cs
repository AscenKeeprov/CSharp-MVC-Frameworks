using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.HTMLConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class UsernameAttribute : DataTypeAttribute, IClientModelValidator
	{
		private string targetDisplayName;

		public UsernameAttribute() : base(DataType.Text) { }

		public void AddValidation(ClientModelValidationContext context)
		{
			if (string.IsNullOrWhiteSpace(targetDisplayName))
			{
				targetDisplayName = context.ModelMetadata.DisplayName ?? context.ModelMetadata.Name;
			}
			if (context.Attributes != null)
			{
				context.Attributes[DataValidationAttributeKey] = true.ToString().ToLower();
				context.Attributes[DataValidationRequiredAttributeKey] = string
					.Format(StringBlankErrorMessage, targetDisplayName);
				context.Attributes[DataValidationMaxLengthAttributeKey] = UsernameMaxLength.ToString();
				context.Attributes[DataValidationMinLengthAttributeKey] = UsernameMinLength.ToString();
				context.Attributes[DataValidationLengthAttributeKey] = string
					.Format(StringLengthErrorMessage, targetDisplayName, UsernameMaxLength, UsernameMinLength);
				context.Attributes[DataValidationRegexPatternAttributeKey] = UsernamePattern;
				context.Attributes[DataValidationRegexAttributeKey] = string
					.Format(FormatMismatchErrorMessage, targetDisplayName);
			}
		}

		public override string FormatErrorMessage(string targetDisplayName)
		{
			return string.Format(InvalidObjectErrorMessage, targetDisplayName);
		}

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			if (string.IsNullOrWhiteSpace(targetDisplayName))
			{
				targetDisplayName = context.DisplayName ?? context.MemberName;
			}
			if (value == null)
			{
				ErrorMessage = string.Format(StringBlankErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			string username = value.ToString();
			if (string.IsNullOrWhiteSpace(username))
			{
				ErrorMessage = string.Format(StringBlankErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			if (username.Length < UsernameMinLength || username.Length > UsernameMaxLength)
			{
				ErrorMessage = string.Format(StringLengthErrorMessage,
					targetDisplayName, UsernameMaxLength, UsernameMinLength);
				return new ValidationResult(ErrorMessage);
			}
			if (!Regex.IsMatch(username, UsernamePattern))
			{
				ErrorMessage = string.Format(FormatMismatchErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			return ValidationResult.Success;
		}
	}
}
