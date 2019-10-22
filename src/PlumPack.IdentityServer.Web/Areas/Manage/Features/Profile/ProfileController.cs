using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlumPack.IdentityServer.Web.Areas.Manage.Features.Profile.Models;
using PlumPack.Web;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.Profile
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var vm = new ProfileViewModel();

            var user = await _userManager.GetUserAsync(User);
            vm.Name = user.Name;
            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            
            user.Name = model.Name;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    AddFailureMessage(error.Description);
                }
            }
            else
            {
                AddSuccessMessage("You profile has been updated.");
            }
            
            return View(model);
        }
    }
}