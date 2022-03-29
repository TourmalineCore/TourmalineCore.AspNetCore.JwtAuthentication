using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLogoutService<TUser> : IdentityLogoutService<TUser, string> where TUser : IdentityUser
    {
        public IdentityLogoutService(IdentityRefreshLoginService<TUser, string> refreshService)
            : base(refreshService)
        {
        }
    }

    internal class IdentityLogoutService<TUser, TKey> : ILogoutService 
        where TUser : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly IdentityRefreshLoginService<TUser, TKey> _refreshService;

        public IdentityLogoutService(IdentityRefreshLoginService<TUser, TKey> refreshService)
        {
            _refreshService = refreshService;
        }

        public async Task LogoutAsync(string userName, LogoutRequestModel model)
        {
            await _refreshService.LogoutAsync(userName, model);
        }
    }
}