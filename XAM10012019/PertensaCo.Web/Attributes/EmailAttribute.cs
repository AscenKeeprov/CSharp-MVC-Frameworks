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
	public class EmailAttribute : DataTypeAttribute, IClientModelValidator
	{
		private string targetDisplayName;

		public EmailAttribute() : base(DataType.EmailAddress) { }

		public void AddValidation(ClientModelValidationContext context)
		{
			if (string.IsNullOrWhiteSpace(targetDisplayName))
			{
				targetDisplayName = context.ModelMetadata.DisplayName ?? context.ModelMetadata.Name;
			}
			if (context.Attributes != null)
			{
				context.Attributes[DataValidationAttributeKey] = true.ToString().ToLower();
				context.Attributes[DataValidationEmailAttributeKey] = string
					.Format(FormatMismatchErrorMessage, targetDisplayName);
				context.Attributes[DataValidationRequiredAttributeKey] = string
					.Format(StringBlankErrorMessage, targetDisplayName);
				context.Attributes[DataValidationMaxLengthAttributeKey] = EmailMaxLength.ToString();
				context.Attributes[DataValidationLengthAttributeKey] = string
					.Format(StringMaxLengthErrorMessage, targetDisplayName, EmailMaxLength);
				context.Attributes[DataValidationRegexPatternAttributeKey] = EmailPattern;
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
			string emailAddress = value.ToString();
			if (string.IsNullOrWhiteSpace(emailAddress))
			{
				ErrorMessage = string.Format(StringBlankErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			if (emailAddress.Length > EmailMaxLength)
			{
				ErrorMessage = string.Format(StringMaxLengthErrorMessage, targetDisplayName, EmailMaxLength);
				return new ValidationResult(ErrorMessage);
			}
			if (!Regex.IsMatch(emailAddress, EmailPattern))
			{
				ErrorMessage = string.Format(FormatMismatchErrorMessage, targetDisplayName);
				return new ValidationResult(ErrorMessage);
			}
			return ValidationResult.Success;
		}
	}
}
