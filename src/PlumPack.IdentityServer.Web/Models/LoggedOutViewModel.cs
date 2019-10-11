namespace PlumPack.IdentityServer.Web.Models
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }
        
        public string SignOutIframeUrl { get; set; }
        
        public string LogoutId { get; set; }
    }
}