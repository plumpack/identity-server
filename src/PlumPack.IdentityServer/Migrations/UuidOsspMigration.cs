using System.Data;
using ServiceStack.OrmLite;
using SharpDataAccess.Migrations;

namespace PlumPack.IdentityServer.Migrations
{
    [Migration]
    public class UuidOsspMigration : IMigration
    {
        public void Run(IDbConnection connection)
        {
            connection.ExecuteSql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
        }

        public int Version => Versions.UuidOssp;
    }
}