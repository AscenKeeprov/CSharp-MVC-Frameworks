using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PertensaCo.Common.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Retrieves an attribute instance of specific type for a given enumeration object.
		/// <para>If the object does not have that attribute, null is returned.</para>
		/// </summary>
		public static TAttribute GetAttribute<TAttribute>(this Enum enumObj)
			where TAttribute : Attribute
		{
			Type enumType = enumObj.GetType();
			string enumName = enumObj.ToString();
			var enumCustomAttributes = enumType
				.GetMember(enumName)[0].GetCustomAttributes();
			var wantedAttribute = enumCustomAttributes
				.FirstOrDefault(a => a is TAttribute);
			return (TAttribute)wantedAttribute;
		}

		public static string GetDisplayName(this Enum enumObj)
		{
			Type enumType = enumObj.GetType();
			string enumName = enumObj.ToString();
			string displayName = enumName;
			var enumCustomAttributes = enumType.GetMember(enumName)[0].GetCustomAttributes();
			foreach (var attribute in enumCustomAttributes)
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

		/// <summary>
		/// Retrieves the string value, numeric value and display name for all fields of an enumeration.
		/// <para>Whenever a display name is not available, it equals the string value.</para>
		/// </summary>
		public static (string Name, object Index, string DisplayName)[] GetExtendedValues(Type enumType)
		{
			var underlyingType = Enum.GetUnderlyingType(enumType);
			var enumValues = Enum.GetValues(enumType).Cast<Enum>();
			var extendedValues = new (string, object, string)[0];
			foreach (var value in enumValues)
			{
				string name = value.ToString();
				object index = Convert.ChangeType(value, underlyingType);
				string displayName = GetDisplayName(value);
				var extendedValue = ValueTuple.Create(name, index, displayName);
				extendedValues = extendedValues.Append(extendedValue).ToArray();
			}
			return extendedValues;
		}

		/// <summary>Check whether an enumeration object has the specified attribute.</summary>
		public static bool HasAttribute<TAttribute>(this Enum enumObj)
			where TAttribute : Attribute
		{
			Type enumType = enumObj.GetType();
			string enumName = enumObj.ToString();
			var enumCustomAttributes = enumType
				.GetMember(enumName)[0].GetCustomAttributes();
			var wantedAttribute = enumCustomAttributes
				.FirstOrDefault(a => a is TAttribute);
			return wantedAttribute != null;
		}
	}
}
