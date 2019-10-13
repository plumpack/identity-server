using System;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PlumPack.IdentityServer.Web.Models;
using PlumPack.Infrastructure.Email;
using ServiceStack;

namespace PlumPack.IdentityServer.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEventService _events;
        private readonly IEmailSender _emailSender;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _userEmailStore;

        public AccountController(IIdentityServerInteractionService interaction,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IEventService events,
            IEmailSender emailSender,
            IUserStore<User> userStore,
            IUserEmailStore<User> userEmailStore)
        {
            _interaction = interaction;
            _signInManager = signInManager;
            _userManager = userManager;
            _events = events;
            _emailSender = emailSender;
            _userStore = userStore;
            _userEmailStore = userEmailStore;
        }
        
        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var model = new RegisterViewModel();
            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = new User();
            await _userStore.SetUserNameAsync(user, model.UserName, CancellationToken.None);
            await _userEmailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                
                var callbackUrl = Url.Page(
                    "/account/confirmemail",
                    pageHandler: null,
                    values: new { userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = user.Email });
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(model.ReturnUrl);
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}