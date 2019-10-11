using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace PlumPack.IdentityServer.ExampleClient.Controllers
{
    [AllowAnonymous]
    public class LogoutController : Controller
    {
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> Index()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
        
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError) throw new Exception(disco.Error);

            var endSessionUrl =
                $"{disco.EndSessionEndpoint}?id_token_hint={idToken}&post_logout_redirect_uri={HttpContext.Request.Scheme}://{HttpContext.Request.Host}/logout/callback";

            return Redirect(endSessionUrl);
        }
        
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> FrontChannel(string sid)
        {
            if (User.IsAuthenticated())
            {
                var currentSid = User.FindFirst("sid")?.Value ?? "";
                if (string.Equals(currentSid, sid, StringComparison.Ordinal))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }

            return NoContent();
        }
        
        public async Task<IActionResult> Callback()
        {
            return Redirect("/");
        }
    }
}