using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlumPack.IdentityServer.Web.Infrastructure;
using PlumPack.Web;

namespace PlumPack.IdentityServer.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            Env = webHostEnvironment;
        }

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Registrar.Register(services, Configuration, "/etc/plumpack/identity-server/");
            
            // Add the services from this assembly.
            PlumPack.Infrastructure.ServiceContext.AddServicesFromAssembly(typeof(Startup).Assembly, services);
            services.AddSingleton(provider => provider.GetRequiredService<KeyStoreFactory>().BuildValidationKeysStore());
            services.AddSingleton(provider => provider.GetRequiredService<KeyStoreFactory>().BuildSigningCredentialStore());
            
            // Configure ASP.NET Identity.
            services.AddIdentity<User, Role>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options => { options.User.RequireUniqueEmail = true; });
            if (Env.IsDevelopment())
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

            // Configure IdentityServer.
            services.Configure<List<ClientApplication>>(Configuration.GetSection("ClientApplications"));
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
                .AddClientStore<ClientStore>()
                .AddAspNetIdentity<User>();
            
            services.AddAuthentication();

            services.PlumPack(Env)
                .AddControllersWithViews()
                .AddValidators(typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.PlumPack(Env)
                .UseExceptionPage()
                .UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseIdentityServer();
            
            app.UseRouting();
            
            app.UseAuthorization();
            app.UseAuthentication();
            
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
