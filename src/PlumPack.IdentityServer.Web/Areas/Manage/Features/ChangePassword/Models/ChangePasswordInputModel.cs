using System.ComponentModel.DataAnnotations;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.ChangePassword.Models
{
    public class ChangePasswordInputModel
    {
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; }
    }
}