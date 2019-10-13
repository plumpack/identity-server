using System.ComponentModel.DataAnnotations;
using PlumPack.IdentityServer.Web.Infrastructure;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.Email.Models
{
    public class EmailInputModel
    {
        [Display(Name = "New email")]
        [EmailAddress]
        public string NewEmail { get; set; }
        
        [Display(Name = "Confirm new email")]
        [EmailAddress]
        public string NewEmailConfirm { get; set; }
        
        [FormValueExists]
        public bool ResendEmailConfirmation { get; set; }
        
        [FormValueExists]
        public bool ChangeEmailAddress { get; set; }
    }
}