using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlumPack.Infrastructure;
using PlumPack.Infrastructure.Data;
using ServiceStack;
using ServiceStack.OrmLite;

namespace PlumPack.IdentityServer.Identity
{
    [Service(typeof(IUserStore<User>))]
    public class UserStore : IUserPasswordStore<User>, IUserEmailStore<User>
    {
        private readonly IDataService _dataService;

        public UserStore(IDataService dataService)
        {
            _dataService = dataService;
        }
        
        #region IUserPasswordStore
        
        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserNameNormalized);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserNameNormalized = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            using (var connection = new ConScope(_dataService))
            {
                using (var transaction = await connection.BeginTransaction())
                {
                    var existing =
                        await connection.Connection.SingleAsync<User>(x => x.EmailNormalized == user.EmailNormalized,
                            cancellationToken);
                    if (existing != null)
                    {
                        return IdentityResult.Failed(new IdentityError
                        {
                            Description = "Email already in use."
                        });
                    }

                    existing = await connection.Connection.SingleAsync<User>(
                        x => x.UserNameNormalized == user.UserNameNormalized, cancellationToken);

                    if (existing != null)
                    {
                        return IdentityResult.Failed(new IdentityError
                        {
                            Description = "User name already in use."
                        });
                    }

                    user.Id = Guid.NewGuid().ToString();
                    await connection.Connection.SaveAsync(user, token: cancellationToken);
                    
                    transaction.Commit();
                    
                    return IdentityResult.Success;
                }
            }
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            using (var connection = _dataService.OpenDbConnection())
            {
                // TODO: Check and make sure the username isn't already in use.
                // TODO: Check and make sure the email isn't already in use.
                await connection.SaveAsync(user, token: cancellationToken);
            }
            
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = new ConScope(_dataService))
            {
                return await connection.Connection.SingleByIdAsync<User>(userId, cancellationToken);
            }
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = new ConScope(_dataService))
            {
                return await connection.Connection.SingleAsync<User>(x => x.UserName.ToUpper() == normalizedUserName, cancellationToken);
            }
        }
        
        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!user.PasswordHash.IsNullOrEmpty());
        }
        
        #endregion
        
        #region IUserEmailStore
        
        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using (var connection = new ConScope(_dataService))
            {
                return await connection.Connection.SingleAsync<User>(x => x.EmailNormalized == normalizedEmail, cancellationToken);
            }
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailNormalized);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailNormalized = normalizedEmail;
            return Task.CompletedTask;
        }
        
        #endregion

        public void Dispose()
        {
            
        }
    }
}