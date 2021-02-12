using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NETCoreTemplate.Entity.Infrastructure;
using NETCoreTemplate.Entity.Models;
using NETCoreTemplate.Entity.Services;
using NETCoreTemplate.Repository.Base;
using NETCoreTemplate.Repository.Context;
using NETCoreTemplate.Service;
using NETCoreTemplate.Web_Razor_.Extensions;

namespace NETCoreTemplate.Web_Razor_
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
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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

            // Services
            services.AddScoped<IWorkService, WorkService>();
            //
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}