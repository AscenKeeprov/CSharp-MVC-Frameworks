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
	public class PersonalNameAttribute : DataTypeAttribute, IClientModelValidator
	{
		private string targetDisplayName;

		public PersonalNameAttribute() : base(DataType.Text) { }

		public void AddValidation(ClientModelValidationContext context)
		{
			if (string.IsNullOrWhiteSpace(targetDisplayName))
			{
				targetDisplayName = context.ModelMetadata.DisplayName ?? context.ModelMetadata.Name;
			}
			if (context.Attributes != null)
			{
				context.Attributes[DataValidationAttributeKey] = true.ToString().ToLower();
				context.Attributes[DataValidationMaxLengthAttributeKey] = PersonalNameMaxLength.ToString();
				context.Attributes[DataValidationLengthAttributeKey] = string
					.Format(StringMaxLengthErrorMessage, targetDisplayName, PersonalNameMaxLength);
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
			if (value != null)
			{
				string personalName = value.ToString();
				if (personalName.Length > PersonalNameMaxLength)
				{
					ErrorMessage = string.Format(StringMaxLengthErrorMessage,
					targetDisplayName, PersonalNameMaxLength);
					return new ValidationResult(ErrorMessage);
				}
				if (!Regex.IsMatch(personalName, PersonalNamePattern))
				{
					ErrorMessage = string.Format(FormatMismatchErrorMessage, targetDisplayName);
					return new ValidationResult(ErrorMessage);
				}
			}
			return ValidationResult.Success;
		}
	}
}
