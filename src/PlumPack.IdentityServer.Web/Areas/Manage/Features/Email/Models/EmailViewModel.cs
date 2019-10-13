using System.ComponentModel.DataAnnotations;

namespace PlumPack.IdentityServer.Web.Areas.Manage.Features.Email.Models
{
    public class EmailViewModel : EmailInputModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public bool IsEmailConfirmed { get; set; }   
    }
}