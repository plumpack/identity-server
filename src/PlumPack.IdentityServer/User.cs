using ServiceStack.DataAnnotations;

namespace PlumPack.IdentityServer
{
    [Alias("users")]
    public class User
    {
        [Alias("id"), PrimaryKey, Required]
        public string Id { get; set; }
        
        [Alias("user_name")]
        public string UserName { get; set; }
        
        [Alias("user_name_normalized")]
        public string UserNameNormalized { get; set; }
        
        [Alias("email")]
        public string Email { get; set; }
        
        [Alias("email_normalized")]
        public string EmailNormalized { get; set; }
        
        [Alias("email_confirmed")]
        public bool EmailConfirmed { get; set; }
        
        [Alias("password_hash")]
        public string PasswordHash { get; set; }
    }
}