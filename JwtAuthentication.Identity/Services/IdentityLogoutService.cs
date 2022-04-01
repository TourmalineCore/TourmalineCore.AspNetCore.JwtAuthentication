using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLogoutService<TUser> : IdentityLogoutService<TUser, string> where TUser : IdentityUser
    {
        public IdentityLogoutService(IRefreshTokenManager<TUser, string> refreshTokenManager)
            : base(refreshTokenManager)
        {
        }
    }

    internal class IdentityLogoutService<TUser, TKey> : ILogoutService 
        where TUser : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly IRefreshTokenManager<TUser, TKey> _refreshTokenManager;

        public IdentityLogoutService(IRefreshTokenManager<TUser, TKey> refreshTokenManager)
        {
            _refreshTokenManager = refreshTokenManager;
        }

        public async Task LogoutAsync(LogoutRequestModel model)
        {
            var user = await _refreshTokenManager.FindRefreshTokenUser(model.RefreshTokenValue, model.ClientFingerPrint);
            await _refreshTokenManager.InvalidateRefreshToken(user.Id, model.RefreshTokenValue);
        }
    }
}