using System.Data;
using PlumPack.Infrastructure.Migrations;
using ServiceStack;
using ServiceStack.OrmLite;

namespace PlumPack.IdentityServer.Migrations
{
    [Migration]
    public class CreateUsers : IMigration
    {
        public void Run(IDbConnection connection)
        {
            connection.ExecuteSql(@"
CREATE TABLE ""users"" 
(
    ""id"" TEXT PRIMARY KEY, 
    ""name"" TEXT NULL,
    ""user_name"" TEXT NOT NULL,
    ""user_name_normalized"" TEXT NOT NULL,
    ""email"" TEXT NOT NULL,
    ""email_normalized"" TEXT NOT NULL,
    ""email_confirmed"" BOOLEAN NOT NULL,
    ""password_hash"" TEXT NULL 
); 
");
        }

        public int Version => Versions.CreateUsers;
    }
}