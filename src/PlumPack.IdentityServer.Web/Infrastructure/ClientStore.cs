using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using PlumPack.Infrastructure;

namespace PlumPack.IdentityServer.Web.Infrastructure
{
    [Service]
    public class ClientStore : IClientStore
    {
        private readonly List<ClientApplication> _clientApplications;

        public ClientStore(IOptions<List<ClientApplication>> clientApplications)
        {
            _clientApplications = clientApplications.Value;
        }
        
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(_clientApplications.SingleOrDefault(x => x.ClientId == clientId)?.BuildIdentityServerClient());
        }
    }
}