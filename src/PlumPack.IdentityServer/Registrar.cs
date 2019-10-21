using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlumPack.Infrastructure.Migrations;

namespace PlumPack.IdentityServer
{
    public class Registrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration, string additionalConfigDirectory)
        {
            // Register our core infrastructure services.
            Infrastructure.Registrar.Register(services, configuration, new MigrationOptions(typeof(Migrations.Versions).Assembly), additionalConfigDirectory);
            
            // Add all the services in this assembly.
            Infrastructure.ServiceContext.AddServicesFromAssembly(typeof(User).Assembly, services);
            services.AddSingleton(context => context.GetRequiredService<IUserStore<User>>() as IUserEmailStore<User>);
            
            // Configure the "SigningOptions".
            services.Configure<SigningOptions>(configuration.GetSection("Signing"));
            var signingYml = Path.Combine(additionalConfigDirectory, "signing.yml");
            if (File.Exists(signingYml))
            {
                services.Configure<SigningOptions>(new ConfigurationBuilder().AddYamlFile(signingYml).Build());
                services.Configure<SigningOptions>(options =>
                    {
                        options.SigningPfx = Infrastructure.InfraHelpers.ResolvePathRelativeToDirectory(options.SigningPfx, additionalConfigDirectory);
                    });
            }
        }
    }
}