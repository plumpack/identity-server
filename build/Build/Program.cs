using Build.Buildary;
using Build.Common;
using static Bullseye.Targets;

namespace Build
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Runner.ParseOptions<Runner.RunnerOptions>(args);
            
            ProjectDefinition.Register(options, new ProjectDefinition
            {
                ProjectName = "Identity Server",
                SolutionPath = "./PlumPack.IdentityServer.sln",
                WebProjectPath = "./src/PlumPack.IdentityServer.Web/PlumPack.IdentityServer.Web.csproj",
                DockerImageName = "plumpack/identity-server"
            });
            
            Runner.Execute(options);
        }
    }
}
