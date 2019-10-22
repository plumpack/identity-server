using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlumPack.IdentityServer.Web.Areas.Manage.Features.ChangePassword.Models;
using PlumPack.Web;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.ChangePassword
{
    [Authorize]
    public class ChangePasswordController : BaseController
    {
        private readonly UserManager<User> _userManager;

        public ChangePasswordController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await BuildViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ChangePasswordInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await BuildViewModel(model));
            }
            
            var user = await _userManager.GetUserAsync(User);

            IdentityResult result;
            if (await _userManager.HasPasswordAsync(user))
            {
                result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }
            else
            {
                result = await _userManager.AddPasswordAsync(user, model.CurrentPassword);
            }
            
            if (result.Succeeded)
            {
                AddSuccessMessage("Your password has been updated.");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    AddFailureMessage(error.Description);
                }
            }
                
            return View(await BuildViewModel(model));
        }

        private async Task<ChangePasswordViewModel> BuildViewModel(ChangePasswordInputModel model = null)
        {
            var user = await _userManager.GetUserAsync(User);

            var vm = new ChangePasswordViewModel
            {
                NewPassword = model?.NewPassword,
                NewPasswordConfirm = model?.NewPasswordConfirm,
                HasPasswordSet = await _userManager.HasPasswordAsync(user)
            };

            if (!vm.HasPasswordSet)
            {
                throw new Exception("Not supported yet, until external logins are added.");
            }
            
            return vm;
        }
    }
}