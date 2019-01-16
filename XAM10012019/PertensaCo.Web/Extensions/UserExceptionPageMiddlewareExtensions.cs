using System;
using Microsoft.AspNetCore.Builder;
using PertensaCo.Web.Middlewares;

namespace PertensaCo.Web.Extensions
{
	public static class UserExceptionPageMiddlewareExtensions
	{
		public static IApplicationBuilder UseClientExceptionPage(this IApplicationBuilder app, string errorPagePath)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}
			if (string.IsNullOrWhiteSpace(errorPagePath))
			{
				throw new ArgumentNullException(nameof(errorPagePath));
			}
			return app.UseMiddleware<UserExceptionPageMiddleware>(errorPagePath);
		}
	}
}
