using System.ComponentModel.DataAnnotations;

namespace PlumPack.IdentityServer.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Display(Name = "Remember my login")]
        public bool RememberLogin { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}