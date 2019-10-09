using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    CreateHostBuilder(o).Build().Run();
                });
        }

        public static IHostBuilder CreateHostBuilder(Options options) =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var urls = options.BuildUrls();
                    if (string.IsNullOrEmpty(urls))
                    {
                        Console.Error.WriteLine("You must provide a port to listen on.");
                        Environment.Exit(1);
                    }
                    Console.WriteLine($"Listening on: {urls}");
                    webBuilder.UseUrls(urls);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
