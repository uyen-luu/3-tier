using System;
using System.Collections.Generic;
using System.IO;
using ApplicationTier.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApplicationTier.WebAPI.Middlewares;
using ApplicationTier.Domain.Models;

namespace ApplicationTier.WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment env)
        {
            Configuration = InitConfiguration(env);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddDatabase()
                .AddServices()
                .AddCORS();

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

        private IConfiguration InitConfiguration(IWebHostEnvironment env)
        {
            // Config the app to read values from appsettings base on current environment value.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables().Build();
            //
            // Map AppSettings section in appsettings.json file value to AppSetting model
            configuration.GetSection("AppSettings").Get<AppSettings>(options => options.BindNonPublicProperties = true);
            return configuration;
        }
    }
}
