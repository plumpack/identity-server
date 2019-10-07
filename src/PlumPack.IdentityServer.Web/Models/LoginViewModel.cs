using ServiceStack.DataAnnotations;

namespace PlumPack.IdentityServer.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public bool RememberLogin { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}