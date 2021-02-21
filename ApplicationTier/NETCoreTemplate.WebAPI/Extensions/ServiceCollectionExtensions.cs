using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCoreTemplate.Domain.Interfaces;
using NETCoreTemplate.Domain.Interfaces.Services;
using NETCoreTemplate.Domain.Models;
using NETCoreTemplate.Infrastructure;
using NETCoreTemplate.Service;

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

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Context
            services.AddDbContext<DemoContext>(options =>
            {
                options.UseSqlServer(AppSettings.ConnectionString,
                    sqlOptions => sqlOptions.CommandTimeout(120));
            });

            // Factory
            services.AddFactory<IUnitOfWork>(serviceProvider => {
                var scopedServiceProvider = serviceProvider.CreateScope().ServiceProvider;
                var dbContext = scopedServiceProvider.GetService<DemoContext>();

                return new UnitOfWork(dbContext);
            });

            return services;
        }

        public static IServiceCollection AddCORS(this IServiceCollection services)
        {
            return // CORS
                services.AddCors(options => {
                    options.AddPolicy("CorsPolicy",
                        builder => {
                            builder.WithOrigins(AppSettings.CORS)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                });
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddScoped<IWorkService, WorkService>();
        }
    }
}
