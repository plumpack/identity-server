using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlumPack.IdentityServer.Web.Infrastructure;

namespace PlumPack.IdentityServer.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Registrar.Register(services, Configuration);
            PlumPack.Infrastructure.ServiceContext.AddServicesFromAssembly(typeof(Startup).Assembly, services);
            services.AddSingleton(provider => provider.GetRequiredService<KeyStoreFactory>().BuildValidationKeysStore());
            services.AddSingleton(provider => provider.GetRequiredService<KeyStoreFactory>().BuildSigningCredentialStore());
            
            services.AddIdentity<User, Role>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => { options.User.RequireUniqueEmail = true; });
            if (WebHostEnvironment.IsDevelopment())
            {
                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequiredUniqueChars = 1;

                    options.User.RequireUniqueEmail = true;
                });
            }
            
            services.Configure<CookieAuthenticationOptions>(
                IdentityConstants.ApplicationScheme,
                options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                });

            var clientApplications = ClientApplication.GetClientApplications(Configuration);
            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.ErrorUrl = "/error";
                    options.UserInteraction.LogoutUrl = "/logout";
                    options.UserInteraction.LoginUrl = "/login";
                    options.UserInteraction.LogoutUrl = "/logout";
                })
                .AddInMemoryIdentityResources(new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email()
                })
                .AddInMemoryClients(clientApplications.Select(x => x.BuildIdentityServerClient()).ToList())
                .AddAspNetIdentity<User>();
            
            services.AddControllersWithViews(options =>
                {
                    // add the "feature" convention
                    options.Conventions.Add(new FeatureConvention());
                    // Auto add [Area("areaName"] to controllers.
                    options.Conventions.Add(new AutoAreaConvention());
                })
                .AddRazorOptions(options =>
                {
                    // using the "feature" convention, expand the paths
                    options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
                })
                .AddFluentValidation()
                .AddRazorRuntimeCompilation();
            services.AddAuthentication();

            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            
            // Add all of our validators
            foreach (var validator in ValidatorDiscovery.DiscoverValidators(typeof(Startup).Assembly))
            {
                services.AddTransient(validator.Interface, validator.Implementation);
            }
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
                app.UseExceptionHandler("/home/error");
            }

            if (env.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = (context) =>
                    {
                        // Disable caching of all static files.
                        context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                        context.Context.Response.Headers["Pragma"] = "no-cache";
                        context.Context.Response.Headers["Expires"] = "-1";
                    }
                });
            }
            else
            {
                app.UseStaticFiles();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("manage", "Manage", "manage/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
