using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlumPack.Infrastructure.Migrations;
using Serilog;
using Serilog.Events;

namespace PlumPack.IdentityServer.Web
{
    public class Program
    {
        public class Options
        {
            [Option('h', "http-port", Default = 5000)]
            public int? HttpPort { get; set; }
            
            [Option('s', "https-port", Default = 5001)]
            public int? HttpsPort { get; set; }

            public string BuildUrls()
            {
                var result = new List<string>();
                if (HttpPort.HasValue)
                {
                    result.Add($"http://localhost:{HttpPort}/");
                }

                if (HttpsPort.HasValue)
                {
                    result.Add($"https://localhost:{HttpsPort}/");
                }

                return string.Join(";", result);
            }
        }
        
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("logging.json", true)
                    .Build())
                .WriteTo.Console()
                .CreateLogger();
            
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    var host = CreateHostBuilder(o).Build();

                    using (var scope = host.Services.CreateScope())
                    {
                        var migrator = scope.ServiceProvider.GetRequiredService<IMigrator>();
                        migrator.Migrate();
                    }
                    
                    using (var scope = host.Services.CreateScope())
                    {
                        var normalizer = scope.ServiceProvider.GetRequiredService<ILookupNormalizer>();
                        var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();
                        var userEmailStore = scope.ServiceProvider.GetRequiredService<IUserEmailStore<User>>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                        
                        Task.Run(async () =>
                        {   
                            var admin = await userStore.FindByNameAsync(normalizer.NormalizeName(User.DefaultUserName), CancellationToken.None);
                            
                            if (admin != null)
                            {
                                // Admin already exists.
                                return;
                            }
                            
                            admin = new User();
                            await userStore.SetUserNameAsync(admin, User.DefaultUserName, CancellationToken.None);
                            await userEmailStore.SetEmailAsync(admin, User.DefaultEmail, CancellationToken.None);
                            await userEmailStore.SetEmailConfirmedAsync(admin, true, CancellationToken.None);
                            var createResult = await userManager.CreateAsync(admin, User.DefaultPassword);
                            
                            if (!createResult.Succeeded)
                            {
                                Log.Logger.Fatal("Couldn't create the default admin user.");
                                Environment.Exit(1);
                            }
                            
                        }).GetAwaiter().GetResult();
                        
                    }

                    host.RunAsync().GetAwaiter().GetResult();
                });
        }

        public static IHostBuilder CreateHostBuilder(Options options) =>
            Host.CreateDefaultBuilder()
                .ConfigureLogging(config =>
                {
                    config.ClearProviders();
                    config.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var urls = options.BuildUrls();
                    if (string.IsNullOrEmpty(urls))
                    {
                        Log.Logger.Fatal("Couldn't create the default admin user.");
                        Environment.Exit(1);
                    }
                    Log.Logger.Information($"Listening on: {urls}");
                    webBuilder.UseUrls(urls);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
