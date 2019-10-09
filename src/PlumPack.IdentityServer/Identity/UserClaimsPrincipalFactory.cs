using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlumPack.Infrastructure;

namespace PlumPack.IdentityServer.Identity
{
    //[Service(typeof(IUserClaimsPrincipalFactory<User>))]
    public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
    {
        public Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return Task.FromResult(new ClaimsPrincipal());
        }
    }
}