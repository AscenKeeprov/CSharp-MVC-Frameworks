using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PertensaCo.Common.Extensions;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Enumerations;
using Serilog;
using Serilog.Events;
using static PertensaCo.Common.Constants.GlobalConstants;

namespace PertensaCo.Web
{
	public class Launcher
	{
		private static ILogger<Launcher> logger;

		public static async Task Main(string[] args)
		{
			try
			{
				var webHost = BuildWebHost(args);
				using (var serviceScope = webHost.Services.CreateScope())
				{
					var services = serviceScope.ServiceProvider;
					logger = services.GetService<ILogger<Launcher>>();
					var dbService = services.GetRequiredService<IDatabaseService>();
					await dbService.InitializeDatabaseAsync();
					await dbService.SeedDatabaseAsync();
				}
				webHost.Run();
			}
			catch (Exception exception)
			{
				logger?.LogCritical(exception.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			var webHostBuilder = WebHost.CreateDefaultBuilder(args)
			.UseStartup<Startup>()
			.UseSerilog((webHostContext, loggerConfiguration) =>
			{
				loggerConfiguration.Enrich.WithThreadId();
				var environment = webHostContext.HostingEnvironment;
				if (environment.IsDevelopment())
				{
					loggerConfiguration.MinimumLevel.Debug();
					loggerConfiguration.MinimumLevel
					.Override(ELogEventSource.System.ToString(), LogEventLevel.Warning);
					loggerConfiguration.MinimumLevel
					.Override(ELogEventSource.Microsoft.ToString(), LogEventLevel.Warning);
				}
				loggerConfiguration.MinimumLevel.Information();
				loggerConfiguration.MinimumLevel
				.Override(ELogEventSource.System.ToString(), LogEventLevel.Error);
				loggerConfiguration.MinimumLevel
				.Override(ELogEventSource.Microsoft.ToString(), LogEventLevel.Error);
				loggerConfiguration.Enrich.FromLogContext();
				if (environment.IsDevelopment())
				{
					loggerConfiguration.WriteTo.Console();
				}
				var logFilePath = string.Format(environment.ContentRootPath +
				LogFilePath.FormatPathSeparators(), environment.EnvironmentName);
				loggerConfiguration.WriteTo.File(
				path: logFilePath,
				outputTemplate: LogEntryTemplate.Replace("{0}", TimestampFormat),
				rollingInterval: RollingInterval.Day);
			});
			var webHost = webHostBuilder.Build();
			return webHost;
		}
	}
}