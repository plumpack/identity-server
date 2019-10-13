using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PlumPack.IdentityServer.Web.Features.Register.Models;
using PlumPack.Infrastructure.Email;

namespace PlumPack.IdentityServer.Web.Features.Register
{
    public class RegisterController : Controller
    {
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _userEmailStore;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<User> _signInManager;

        public RegisterController(IUserStore<User> userStore,
            IUserEmailStore<User> userEmailStore,
            UserManager<User> userManager,
            IEmailSender emailSender,
            SignInManager<User> signInManager)
        {
            _userStore = userStore;
            _userEmailStore = userEmailStore;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }
        
        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            var model = new RegisterViewModel();
            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
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
                
                var callbackUrl = Url.Action("Index", "ConfirmEmail", new
                    {
                        area = "",
                        userId, code
                    }, Request.Scheme);

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