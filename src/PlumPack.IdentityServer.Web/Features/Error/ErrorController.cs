using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PlumPack.Web.Models;

namespace PlumPack.IdentityServer.Web.Features.Error
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;

        public ErrorController(IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment)
        {
            _interaction = interaction;
            _environment = environment;
        }
        
        public async Task<IActionResult> Index(string errorId)
        {
            var vm = new ErrorViewModel();

            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message.Error;

                if (_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = message.ErrorDescription;
                }
            }

            return View("Error", vm);
        }
    }
}