using System;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web.Extensions
{
	public static class HttpContextExtensions
	{
		public static string GenerateErrorId(this HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			string connectionId = context.Connection?.Id;
			string timestamp = DateTimeOffset.Now
				.ToString(TimestampFormat, CultureInfo.InvariantCulture)
				.Replace(' ', '_');
			if (string.IsNullOrWhiteSpace(connectionId))
			{
				return $"EID_{timestamp}";
			}
			else return $"EID_{timestamp}_{connectionId}";
		}

		public static CultureInfo GetRequestCultureInfo(this HttpContext context)
		{
			var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
			if (requestCultureFeature == null) return CultureInfo.InstalledUICulture;
			var requestCulture = requestCultureFeature.RequestCulture;
			CultureInfo requestCultureInfo = requestCulture.Culture;
			return requestCultureInfo;
		}

		public static string GetUserId(this HttpContext context)
		{
			string userId = context.User.GetId();
			if (string.IsNullOrWhiteSpace(userId) && context.Session.IsAvailable)
			{
				if (context.Session.TryGetValue(UserIdKey, out byte[] userIdBytes))
				{
					userId = Encoding.UTF8.GetString(userIdBytes, 0, userIdBytes.Length);
				}
			}
			return userId;
		}
	}
}
