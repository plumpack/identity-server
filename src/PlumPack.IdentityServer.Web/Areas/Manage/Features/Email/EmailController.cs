using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PlumPack.IdentityServer.Web.Areas.Manage.Features.Email.Models;
using PlumPack.Infrastructure.Email;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.Email
{
    public class EmailController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public EmailController(UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await BuildViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EmailInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await BuildViewModel(model));
            }

            if (model.ResendEmailConfirmation)
            {
                var user = await _userManager.GetUserAsync(User);

                var userId = await _userManager.GetUserIdAsync(user);
                var email = await _userManager.GetEmailAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                
                var callbackUrl = Url.Action("Index", "ConfirmEmail", new
                    {
                        area = "",
                        userId, code
                    }, Request.Scheme);
                
                await _emailSender.SendEmailAsync(
                    email,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                AddSuccessMessage("Verification email sent. Please check your email.");
            }

            if (model.ChangeEmailAddress)
            {
               AddSuccessMessage("TODO");
            }

            return View(await BuildViewModel(model));
        }

        private async Task<EmailViewModel> BuildViewModel(EmailInputModel input = null)
        {
            var vm = new EmailViewModel
            {
                NewEmail = input?.NewEmail,
                NewEmailConfirm = input?.NewEmailConfirm
            };
            
            var user = await _userManager.GetUserAsync(User);

            vm.Email = user.Email;
            vm.IsEmailConfirmed = user.EmailConfirmed;

            return vm;
        }
    }
}