using ServiceStack.DataAnnotations;

namespace PlumPack.IdentityServer
{
    public class User
    {
        [Alias("id"), PrimaryKey, Required]
        public string Id { get; set; }
        
        public string Username { get; set; }
    }
}