using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshTokenManager<TUser> : RefreshTokenManager<TUser, string> where TUser : IdentityUser
    {
        public RefreshTokenManager(TourmalineDbContext<TUser, string> dbContext, RefreshAuthenticationOptions options)
            : base(dbContext, options)
        {
        }
    }

    internal class RefreshTokenManager<TUser, TKey> : IRefreshTokenManager where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly TourmalineDbContext<TUser, TKey> _dbContext;
        private readonly RefreshAuthenticationOptions _options;

        public RefreshTokenManager(
            TourmalineDbContext<TUser, TKey> dbContext,
            RefreshAuthenticationOptions options)
        {
            _dbContext = dbContext;
            _options = options;
        }

        public async Task<TokenModel> GetRefreshToken(object user, string clientFingerPrint)
        {
            var refreshToken = CreateRefreshToken(user, clientFingerPrint);

            _dbContext.Attach(refreshToken.User);
            await _dbContext.Set<RefreshToken<TUser, TKey>>().AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new TokenModel
            {
                Value = refreshToken.Value.ToString(),
                ExpiresInUtc = refreshToken.ExpiresIn.ToUniversalTime(),
            };
        }

        private RefreshToken<TUser, TKey> CreateRefreshToken(object user, string clientFingerPrint)
        {
            var expiresDate = DateTime.UtcNow.AddMinutes(_options.RefreshTokenExpireInMinutes);

            var newToken = new RefreshToken<TUser, TKey>
            {
                Value = Guid.NewGuid(),
                ExpiresIn = expiresDate,
                IsActive = true,
                ClientFingerPrint = clientFingerPrint,
                User = (TUser)user,
            };

            return newToken;
        }
    }
}