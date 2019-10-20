using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlumPack.Infrastructure;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    [Service]
    public class KeyStoreFactory
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SigningOptions _options;

        public KeyStoreFactory(IOptions<SigningOptions> options, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _options = options.Value;
        }
        
        public IValidationKeysStore BuildValidationKeysStore()
        {
            var tmp = new Temp();

            if (string.IsNullOrEmpty(_options.SigningPfx) && _webHostEnvironment.IsDevelopment())
            {
                tmp.AddDeveloperSigningCredential();
            }
            else if (string.IsNullOrEmpty(_options.SigningPfx))
            {
                throw new Exception("No signing pfx file provided.");
            }
            else
            {
                if (!File.Exists(_options.SigningPfx))
                {
                    throw new Exception($"Signing file {_options.SigningPfx} doesn't exist.");
                }

                tmp.AddSigningCredential(new X509Certificate2(_options.SigningPfx));
            }
            
            using (var provider = tmp.Services.BuildServiceProvider())
            {
                return provider.GetRequiredService<IValidationKeysStore>();
            }
        }

        public ISigningCredentialStore BuildSigningCredentialStore()
        {
            var tmp = new Temp();

            if (string.IsNullOrEmpty(_options.SigningPfx) && _webHostEnvironment.IsDevelopment())
            {
                tmp.AddDeveloperSigningCredential();
            }
            else if (string.IsNullOrEmpty(_options.SigningPfx))
            {
                throw new Exception("No signing pfx file provided.");
            }
            else
            {
                if (!File.Exists(_options.SigningPfx))
                {
                    throw new Exception($"Signing file {_options.SigningPfx} doesn't exist.");
                }

                tmp.AddSigningCredential(new X509Certificate2(_options.SigningPfx));
            }
            
            using (var provider = tmp.Services.BuildServiceProvider())
            {
                return provider.GetRequiredService<ISigningCredentialStore>();
            }
        }
        
        class Temp : IIdentityServerBuilder
        {
            public IServiceCollection Services { get; } = new ServiceCollection();
        }
    }
}