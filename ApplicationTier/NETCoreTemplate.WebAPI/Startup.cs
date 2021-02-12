using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCoreTemplate.Entity.Infrastructure;
using NETCoreTemplate.Entity.Models;
using NETCoreTemplate.Entity.Services;
using NETCoreTemplate.Repository.Base;
using NETCoreTemplate.Repository.Context;
using NETCoreTemplate.Service;
using NETCoreTemplate.WebAPI.Extensions;
using NETCoreTemplate.WebAPI.Middlewares;

namespace NETCoreTemplate.WebAPI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables().Build();
            //
            // Map AppSettings section in appsettings.json file value to static classes
            configuration.GetSection("AppSettings").Get<AppSettings>(options => options.BindNonPublicProperties = true);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

            // Services
            services.AddScoped<IWorkService, WorkService>();

            // CORS
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => {
                        builder.WithOrigins(AppSettings.CORS)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvcCore(option =>
            {
                option.RespectBrowserAcceptHeader = true;
                option.EnableEndpointRouting = false;
            }).AddJsonOptions(option => { option.JsonSerializerOptions.WriteIndented = true; });
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value)
                                                       && !context.Request.Path.Value.StartsWith("/api"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseCors("CorsPolicy");
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } });
            app.UseStaticFiles();
            app.UseMvc();
            app.UseHealthChecks("/healthcheck");
        }
    }
}
