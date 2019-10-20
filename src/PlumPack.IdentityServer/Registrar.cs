using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlumPack.Infrastructure.Migrations;

namespace PlumPack.IdentityServer
{
    public class Registrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            Infrastructure.ServiceContext.AddServicesFromAssembly(typeof(User).Assembly, services);
            services.AddSingleton(context => context.GetRequiredService<IUserStore<User>>() as IUserEmailStore<User>);
            Infrastructure.Registrar.Register(services, configuration, new MigrationOptions(typeof(Migrations.Versions).Assembly), "/etc/plumpack/identity-server/");

            services.Configure<SigningOptions>(configuration.GetSection("Signing"));
                
            var signingYml = "/etc/plumpack/identity-server/signing.yml";
            if (File.Exists(signingYml))
            {
                services.Configure<SigningOptions>(new ConfigurationBuilder().AddYamlFile(signingYml).Build());
                services.Configure<SigningOptions>(options =>
                    {
                        options.SigningPfx = Infrastructure.InfraHelpers.ResolvePathRelativeToDirectory(options.SigningPfx, "/etc/plumpack/identity-server/");
                    });
            }
        }
    }
}