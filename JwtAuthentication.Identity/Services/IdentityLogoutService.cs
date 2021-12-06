using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLogoutService<TUser> : ILogoutService where TUser : IdentityUser
    {
        private readonly RefreshSignInManager<TUser> _signInManager;

        public IdentityLogoutService(RefreshSignInManager<TUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task LogoutAsync(LogoutRequestModel model)
        {
            await _signInManager.InvalidateRefreshTokenForUser(model.RefreshTokenValue, model.ClientFingerPrint);
        }
    }

    internal class IdentityLogoutService<TUser, TKey> : ILogoutService where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly RefreshSignInManager<TUser, TKey> _signInManager;

        public IdentityLogoutService(RefreshSignInManager<TUser, TKey> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task LogoutAsync(LogoutRequestModel model)
        {
            await _signInManager.InvalidateRefreshTokenForUser(model.RefreshTokenValue, model.ClientFingerPrint);
        }
    }
}