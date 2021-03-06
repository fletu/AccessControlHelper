﻿using AccessControlDemo.Database;
using AccessControlDemo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControlDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    //options.AccessDeniedPath = "/Account/Login";
                    options.LoginPath = "/Account/Login";

                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                });

            // Add framework services.
            services.AddMvc();

            services.AddScoped<PermissionsDbContext>();

            //services.TryAddScoped<IResourceAccessStrategy, ActionAccessStrategy>();
            //services.TryAddSingleton<IControlAccessStrategy, ControlAccessStrategy>();
            //services.AddAccessControlHelper();

            services.AddAccessControlHelper<ActionAccessStrategy, ControlAccessStrategy>(ServiceLifetime.Scoped, ServiceLifetime.Singleton);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseAuthentication();
            // UseAccessControlHelper  for global authorization if needed
            //app.UseAccessControlHelper();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
