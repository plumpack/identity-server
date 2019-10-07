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
CREATE TABLE ""user"" 
(
    ""id"" TEXT PRIMARY KEY, 
    ""username"" TEXT NULL 
); 
");
        }

        public int Version => Versions.CreateUsers;
    }
}