using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PlumPack.IdentityServer.Web.Areas.Manage.Features.Email.Models;
using PlumPack.Infrastructure.Email;
using PlumPack.Web;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.Email
{
    [Authorize]
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
                    new MailAddress(email), 
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                AddSuccessMessage("Verification email sent. Please check your email.");
            }

            if (model.ChangeEmailAddress)
            {
                var user = await _userManager.GetUserAsync(User);
                
                
                if (model.NewEmail != user.Email)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("Change", "ConfirmEmail", new
                    {
                        area = "",
                        userId, code, email = model.NewEmail
                    }, Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        new MailAddress(model.NewEmail), 
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    AddSuccessMessage("Confirmation link to change email sent. Please check your email.");
                    
                    return View(await BuildViewModel());
                }

                AddFailureMessage("Your email is unchanged.");

                return View(await BuildViewModel());
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