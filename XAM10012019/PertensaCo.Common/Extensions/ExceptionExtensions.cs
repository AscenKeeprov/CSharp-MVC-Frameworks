using System;
using System.Text;

namespace PertensaCo.Common.Extensions
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Uses a given new line separator to concatenate all messages in an exception stack.
		///  Messages are categorized by exception type.
		/// <para>If no separator is specified, the default one for the current platform is used.</para>
		/// </summary>
		public static string GetMessageStack(this Exception exception, string newLine = null)
		{
			if (exception == null) return string.Empty;
			string message = exception.ParseExceptionMessage();
			StringBuilder messageStack = new StringBuilder(message);
			if (string.IsNullOrWhiteSpace(newLine)) newLine = Environment.NewLine;
			while (exception.InnerException != null)
			{
				message = exception.InnerException.ParseExceptionMessage();
				messageStack.Append(newLine);
				messageStack.Append(message);
				exception = exception.InnerException;
			}
			return messageStack.ToString();
		}

		public static string ParseExceptionMessage(this Exception exception)
		{
			string exceptionType = exception.GetType().Name;
			string exceptionMessage = exception.Message;
			string parsedMessage = $"[{exceptionType}] {exceptionMessage}";
			return parsedMessage;
		}
	}
}
