using System.Data;
using PlumPack.Infrastructure.Migrations;
using ServiceStack;
using ServiceStack.OrmLite;
using SharpDataAccess.Migrations;

namespace PlumPack.IdentityServer.Migrations
{
    [Migration]
    public class CreateUsers : IMigration
    {
        public void Run(IDbConnection connection)
        {
            connection.CreateTable<User>();
        }

        public int Version => Versions.CreateUsers;
    }
}