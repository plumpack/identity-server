using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    public class ClientApplication
    {
        public string ClientId { get; set; }
            
        public string ClientName { get; set; }
        
        public string BaseUrl { get; set; }
            
        public string Secret { get; set; }
            
        public List<string> RedirectUris { get; set; }
            
        public List<string> PostLogoutRedirectUris { get; set; }
        
        public string BackChannelLogoutUri { get; set; }
        
        public string FrontChannelLogoutUri { get; set; }

        public Client BuildIdentityServerClient()
        {
            return new Client
            {
                ClientId = ClientId,
                ClientName = ClientName,
                AllowedGrantTypes = GrantTypes.Hybrid,

                ClientSecrets =
                {
                    new Secret(Secret.Sha256())
                },

                RedirectUris = RedirectUris,
                PostLogoutRedirectUris = PostLogoutRedirectUris,
                BackChannelLogoutUri = BackChannelLogoutUri,
                FrontChannelLogoutUri = FrontChannelLogoutUri,

                RequireConsent = false,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email
                },

                AllowOfflineAccess = true
            };
        }
    }
}