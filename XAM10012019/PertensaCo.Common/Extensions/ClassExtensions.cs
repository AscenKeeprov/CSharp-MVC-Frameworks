using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PertensaCo.Common.Extensions
{
	public static class ClassExtensions
	{
		public static string GetPropertyDisplayName(this object classObj, string propertyName)
		{
			PropertyInfo property = classObj.GetType().GetProperty(propertyName);
			string displayName = propertyName;
			var attributes = property.GetCustomAttributes();
			foreach (var attribute in attributes)
			{
				if (attribute is DisplayNameAttribute displayNameAttribute
					&& !string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName))
				{
					displayName = displayNameAttribute.DisplayName;
					break;
				}
				if (attribute is DisplayAttribute displayAttribute
					&& !string.IsNullOrWhiteSpace(displayAttribute.Name))
				{
					displayName = displayAttribute.Name;
					break;
				}
			}
			return displayName;
		}
	}
}
