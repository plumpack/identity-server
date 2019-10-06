using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PlumPack.IdentityServer.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (DictionaryEntry env in System.Environment.GetEnvironmentVariables())
            {
                
                Console.WriteLine($"{env.Key}:{env.Value}");
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://localhost:5001;https://localhost:5002");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
