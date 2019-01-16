using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PertensaCo.Web.Extensions;

namespace PertensaCo.Web.Middlewares
{
	public class UserExceptionPageMiddleware
	{
		private readonly PathString errorPagePath;
		private readonly RequestDelegate next;
		private readonly ILogger<UserExceptionPageMiddleware> logger;

		public UserExceptionPageMiddleware(RequestDelegate next, ILogger<UserExceptionPageMiddleware> logger, string errorPagePath)
		{
			this.errorPagePath = new PathString(errorPagePath);
			this.logger = logger;
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, IHostingEnvironment environment)
		{
			try
			{
				await next(context);
			}
			catch (Exception exception)
			{
				var message = $"[ID: {context.Connection.Id}] [REQ: {context.Request.Path} {context.Request.Method}]";
				logger.LogError(exception.GetBaseException(), message);
				if (context.Response.HasStarted)
				{
					var errorId = context.GenerateErrorId();
					StringBuilder content = new StringBuilder(Environment.NewLine);
					content.AppendLine("An error occurred while processing your request.");
					content.AppendLine("Please contact your administrator with the information provided below to get help.");
					content.AppendLine($"Error ID: {errorId}");
					await context.Response.WriteAsync(content.ToString());
				}
				else context.Response.Redirect(errorPagePath);
			}
		}
	}
}
