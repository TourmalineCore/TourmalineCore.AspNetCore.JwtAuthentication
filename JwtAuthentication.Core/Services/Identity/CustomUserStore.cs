using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity
{
    public class CustomUserStore<TUser> : IUserStore<TUser>
        where TUser : User
    {
        private readonly Dictionary<string, TUser> _users = new Dictionary<string, TUser>();

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            var isUserCreated = _users.TryAdd(user.UserName, user);

            return isUserCreated
                ? Task.FromResult(IdentityResult.Success)
                : Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Create user",
                                Description = "User already exist",
                            }
                        )
                    );
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var isUserExist = _users.TryGetValue(user.UserName, out _);

            if (!isUserExist)
            {
                return Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Remove user",
                                Description = "Invalid user name",
                            }
                        )
                    );
            }

            _users[user.UserName] = user;

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            if (_users.Count == 0)
            {
                return null;
            }

            var isUserDeleted = _users.Remove(user.UserName);

            return isUserDeleted
                ? Task.FromResult(IdentityResult.Success)
                : Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Remove user",
                                Description = "Invalid user id",
                            }
                        )
                    );
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.Values.FirstOrDefault(x => x.Id == userId));
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.Values.FirstOrDefault(x => x.NormalizedUserName == normalizedUserName));
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName = normalizedName);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash = passwordHash);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName = userName);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}