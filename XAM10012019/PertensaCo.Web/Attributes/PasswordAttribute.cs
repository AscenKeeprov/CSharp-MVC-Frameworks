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
	public class PasswordAttribute : DataTypeAttribute, IClientModelValidator
	{
		private string targetDisplayName = "Password";

		public PasswordAttribute() : base(DataType.Password) { }

		public void AddValidation(ClientModelValidationContext context)
		{
			if (!string.IsNullOrWhiteSpace(context.ModelMetadata.DisplayName)
				&& context.ModelMetadata.DisplayName != context.ModelMetadata.Name)
			{
				targetDisplayName = context.ModelMetadata.DisplayName;
			}
			if (context.Attributes != null)
			{
				context.Attributes[DataValidationAttributeKey] = true.ToString().ToLower();
				context.Attributes[DataValidationRequiredAttributeKey] = string
					.Format(StringBlankErrorMessage, targetDisplayName);
				context.Attributes[DataValidationMinLengthAttributeKey] = PasswordMinLength.ToString();
				context.Attributes[DataValidationLengthAttributeKey] = string
					.Format(StringMinLengthErrorMessage, targetDisplayName, PasswordMinLength);
				context.Attributes[DataValidationRegexPatternAttributeKey] = PasswordPattern;
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
			if (!string.IsNullOrWhiteSpace(context.DisplayName) 
				&& context.DisplayName != context.MemberName)
			{
				targetDisplayName = context.DisplayName;
			}
			if (value == null)
			{
				ErrorMessage = string.Format(StringBlankErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			string password = value.ToString();
			if (string.IsNullOrWhiteSpace(password))
			{
				ErrorMessage = string.Format(StringBlankErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			if (password.Length < PasswordMinLength)
			{
				ErrorMessage = string.Format(StringMinLengthErrorMessage, targetDisplayName, PasswordMinLength);
				return new ValidationResult(ErrorMessage);
			}
			if (!Regex.IsMatch(password, PasswordPattern))
			{
				ErrorMessage = string.Format(FormatMismatchErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			return ValidationResult.Success;
		}
	}
}
