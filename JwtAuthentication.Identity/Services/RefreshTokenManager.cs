using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshTokenManager<TUser> : TokenManager where TUser : IdentityUser
    {
        private readonly AuthenticationWithRefreshOptions _options;

        public RefreshTokenManager(
            IOptions<AuthenticationWithRefreshOptions> options,
            IUserClaimsProvider userClaimsProvider) : base(options, userClaimsProvider)
        {
            _options = options.Value;
        }

        public RefreshToken<TUser> GetRefreshToken(TUser user, string clientFingerPrint)
        {
            var expiresDate = DateTime.UtcNow.AddMinutes(_options.RefreshTokenExpireInMinutes);
            var newToken = new RefreshToken<TUser>
            {
                Value = Guid.NewGuid(),
                ExpiresIn = expiresDate,
                IsActive = true,
                ClientFingerPrint = clientFingerPrint,
                User = user,
            };

            return newToken;
        }
    }
}