using System.Collections.Generic;
using Flurl;
using Microsoft.Extensions.Options;
using PlumPack.Infrastructure;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    [Service(typeof(IPostConfigureOptions<List<ClientApplication>>))]
    public class ClientApplicationsConfigure : IPostConfigureOptions<List<ClientApplication>>
    {
        public void PostConfigure(string name, List<ClientApplication> options)
        {
            foreach (var client in options)
            {
                if (!string.IsNullOrEmpty(client.BaseUrl))
                {
                    if (client.RedirectUris == null || client.RedirectUris.Count == 0)
                    {
                        client.RedirectUris = new List<string>
                        {
                           Url.Combine(client.BaseUrl, "signin-oidc")
                        };
                    }
                    if (client.PostLogoutRedirectUris == null || client.PostLogoutRedirectUris.Count == 0)
                    {
                        client.PostLogoutRedirectUris = new List<string>
                        {
                            client.BaseUrl,
                            Url.Combine(client.BaseUrl, "logout", "frontchannel"),
                            Url.Combine(client.BaseUrl, "logout", "callback")
                        };
                    }
                    if (string.IsNullOrEmpty(client.FrontChannelLogoutUri))
                    {
                        client.FrontChannelLogoutUri = Url.Combine(client.BaseUrl, "logout", "frontchannel");
                    }
                }
            }
        }
    }
}