using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity
{
    public class CustomUserStore<TUser> : IUserPasswordStore<TUser, long>
        where TUser : User
    {
        private static readonly Dictionary<long, TUser> Users = new Dictionary<long, TUser>();

        private bool _disposedValue;

        public Task CreateAsync(TUser user)
        {
            user.Id = Users.Keys.Last() + 1;

            var isSuccess = Users.TryAdd(user.Id, user);

            return isSuccess == false
                ? Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Registration",
                                Description = "Invalid user id",
                            }
                        )
                    )
                : Task.FromResult(IdentityResult.Success);
        }

        public Task DeleteAsync(TUser user)
        {
            var isSuccess = Users.Remove(user.Id);

            return isSuccess == false
                ? Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Remove user",
                                Description = "Invalid user id",
                            }
                        )
                    )
                : Task.FromResult(IdentityResult.Success);
        }

        public Task<TUser> FindByIdAsync(long userId)
        {
            return Task.FromResult(Users.Values.FirstOrDefault(x => x.Id == userId));
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName)
        {
            return Task.FromResult(Users.Values.FirstOrDefault(x => x.NormalizedUserName == normalizedUserName));
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<long> GetUserIdAsync(TUser user)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName)
        {
            return Task.FromResult(user.NormalizedUserName = normalizedName);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            return Task.FromResult(user.PasswordHash = passwordHash);
        }

        public Task SetUserNameAsync(TUser user, string userName)
        {
            return Task.FromResult(user.UserName = userName);
        }

        public Task UpdateAsync(TUser user)
        {
            var isUserExist = Users.TryGetValue(user.Id, out _);

            if (!isUserExist)
            {
                return Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Remove user",
                                Description = "Invalid user id",
                            }
                        )
                    );
            }

            Users[user.Id] = user;

            return Task.FromResult(IdentityResult.Success);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CustomUserStore()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}