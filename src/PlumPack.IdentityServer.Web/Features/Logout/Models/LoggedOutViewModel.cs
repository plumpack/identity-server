namespace PlumPack.IdentityServer.Web.Features.Logout.Models
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }
        
        public string SignOutIframeUrl { get; set; }
        
        public string LogoutId { get; set; }
    }
}