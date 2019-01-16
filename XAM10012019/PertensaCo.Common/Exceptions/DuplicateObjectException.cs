using System;

namespace PertensaCo.Common.Exceptions
{
	public class DuplicateObjectException : Exception
	{
		private const string messagePattern = "{0} '{1}' already exists.";

		private readonly string objectType;
		private readonly string objectName;

		public DuplicateObjectException(string objectType, string objectName)
			: base(string.Format(messagePattern, objectType, objectName))
		{
			this.objectType = objectType;
			this.objectName = objectName;
		}

		public DuplicateObjectException(Type objectType, string objectName)
			: this(objectType.Name, objectName) { }

		public override string Message => string
			.Format(messagePattern, objectType, objectName);
	}
}
