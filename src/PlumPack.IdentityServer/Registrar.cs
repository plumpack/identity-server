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
            Infrastructure.Registrar.Register(services, configuration, new MigrationOptions(typeof(Migrations.Versions).Assembly));
        }
    }
}