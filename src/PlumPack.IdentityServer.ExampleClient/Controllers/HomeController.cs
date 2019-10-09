using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlumPack.IdentityServer.ExampleClient.Models;

namespace PlumPack.IdentityServer.ExampleClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }
        
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
