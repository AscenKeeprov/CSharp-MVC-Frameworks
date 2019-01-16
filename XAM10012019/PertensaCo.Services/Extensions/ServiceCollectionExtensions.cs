using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PertensaCo.Services.Contracts;

namespace PertensaCo.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
	public static IServiceCollection AddMessenger(this IServiceCollection services, Action<IMessengerOptions> options)
	{
	    if (services == null)
	    {
		throw new ArgumentNullException(nameof(services));
	    }
	    services.TryAddScoped<IMessengerService, MessengerService>();
	    if (options != null)
	    {
		services.Configure<MessengerOptions>(options);
	    }
	    return services;
	}
    }
}
