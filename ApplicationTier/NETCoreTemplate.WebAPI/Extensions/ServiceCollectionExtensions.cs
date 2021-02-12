using System;
using Microsoft.Extensions.DependencyInjection;

namespace NETCoreTemplate.WebAPI.Extensions
{
	public static class ServiceCollectionExtensions
    {
        public static void AddFactory<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>());
        }

        public static void AddFactory<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            services.AddTransient(implementationFactory);
            services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>());
        }
	}
}
