namespace PlumPack.IdentityServer.Web.Features.Logout.Models
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}