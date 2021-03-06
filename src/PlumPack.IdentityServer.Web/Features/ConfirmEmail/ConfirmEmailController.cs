using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PlumPack.IdentityServer.Web.Features.ConfirmEmail.Model;
using PlumPack.Infrastructure;
using PlumPack.Web;

namespace PlumPack.IdentityServer.Web.Features.ConfirmEmail
{
    public class ConfirmEmailController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly PlumPackOptions _plumPackOptions;

        public ConfirmEmailController(UserManager<User> userManager,
            IOptions<PlumPackOptions> plumPackOptions)
        {
            _userManager = userManager;
            _plumPackOptions = plumPackOptions.Value;
        }
        
        public async Task<ActionResult> Index(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return Redirect(_plumPackOptions.MainSiteUrl);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            var message = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            
            // Check to see if the logged in user is the user we confirmed the email for.
            // If so, redirect to the email page, showing a notification.
            if (User.IsAuthenticated())
            {
                var loggedInUser = await _userManager.GetUserAsync(User);
                if (loggedInUser.Id == user.Id)
                {
                    // Same user.
                    if (result.Succeeded)
                    {
                        AddSuccessMessage(message, true);
                    }
                    else
                    {
                        AddFailureMessage(message, true);
                    }
                    return RedirectToAction("Index", "Email", new {area = "Manage"});
                }
            }
            
            var vm = new ConfirmEmailViewModel();
            vm.Status = result.Succeeded;
            vm.StatusMessage = message;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Change(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return Redirect(_plumPackOptions.MainSiteUrl);
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);

            var message = result.Succeeded ? "Thank you for confirming your email change." : "Error changing email.";
            
            // Check to see if the logged in user is the user we confirmed the email for.
            // If so, redirect to the email page, showing a notification.
            if (User.IsAuthenticated())
            {
                var loggedInUser = await _userManager.GetUserAsync(User);
                if (loggedInUser.Id == user.Id)
                {
                    // Same user.
                    if (result.Succeeded)
                    {
                        AddSuccessMessage(message, true);
                    }
                    else
                    {
                        AddFailureMessage(message, true);
                    }
                    return RedirectToAction("Index", "Email", new {area = "Manage"});
                }
            }
            
            var vm = new ConfirmEmailViewModel();
            vm.Status = result.Succeeded;
            vm.StatusMessage = message;

            return View(vm);
        }
    }
}