using System;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace PlumPack.IdentityServer.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PlumPack.Web.Main.Program.Main<Startup>(5000, "/etc/plumpack/identity-server/", args, async host =>
            {
                using (var scope = host.Services.CreateScope())
                {
                    var normalizer = scope.ServiceProvider.GetRequiredService<ILookupNormalizer>();
                    var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();
                    var userEmailStore = scope.ServiceProvider.GetRequiredService<IUserEmailStore<User>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    var admin = await userStore.FindByNameAsync(normalizer.NormalizeName(User.DefaultUserName),
                        CancellationToken.None);

                    if (admin != null)
                    {
                        // Admin already exists.
                        return;
                    }

                    admin = new User();
                    await userStore.SetUserNameAsync(admin, User.DefaultUserName, CancellationToken.None);
                    await userEmailStore.SetEmailAsync(admin, User.DefaultEmail, CancellationToken.None);
                    await userEmailStore.SetEmailConfirmedAsync(admin, true, CancellationToken.None);
                    var createResult = await userManager.CreateAsync(admin, User.DefaultPassword);

                    if (!createResult.Succeeded)
                    {
                        Log.Logger.Fatal("Couldn't create the default admin user.");
                        Environment.Exit(1);
                    }
                }
            });
        }
    }
}
