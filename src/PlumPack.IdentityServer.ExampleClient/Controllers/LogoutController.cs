using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace PlumPack.IdentityServer.ExampleClient.Controllers
{
    public class LogoutController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");
        
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError) throw new Exception(disco.Error);

            var endSessionUrl =
                $"{disco.EndSessionEndpoint}?id_token_hint={idToken}&post_logout_redirect_uri={HttpContext.Request.Scheme}://{HttpContext.Request.Host}/logout/backchannel";

            return Redirect(endSessionUrl);
        }
        
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> BackChannel(string logout_token)
        {
            try
            {
//                var user = await ValidateLogoutToken(logout_token);
//
//                // these are the sub & sid to signout
//                var sub = user.FindFirst("sub")?.Value;
//                var sid = user.FindFirst("sid")?.Value;
//
//                LogoutSessions.Add(sub, sid);

                return Ok();
            }
            catch { }

            return BadRequest();
        }
    }
}