using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

[assembly: InternalsVisibleTo("Tests")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshTokenManager<TUser> : RefreshTokenManager<TUser, string> where TUser : IdentityUser
    {
        public RefreshTokenManager(
            TourmalineDbContext<TUser, string> dbContext,
            RefreshAuthenticationOptions options)
        :base(dbContext, options)
        {
        }
    }

    internal class RefreshTokenManager<TUser, TKey> : IRefreshTokenManager<TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
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

        public async Task<TokenModel> GenerateRefreshToken(object user, string clientFingerPrint)
        {
            var refreshToken = CreateRefreshToken(user, clientFingerPrint);

            _dbContext.Attach(refreshToken.User);
            await _dbContext.Set<RefreshToken<TUser, TKey>>().AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return BuildTokenModelByRefreshToken(refreshToken);
        }

        public async Task<bool> IsTokenAlreadyInvalidated(TKey userId, Guid refreshTokenValue)
        {
            var token = await _dbContext
                .Set<RefreshToken<TUser, TKey>>()
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Value == refreshTokenValue && x.UserId.Equals(userId));

            ThrowExceptionIfTokenIsNull(token);

            return !token.IsActive;
        }

        public async Task InvalidateRefreshToken(TKey userId, Guid refreshTokenValue)
        {
            var token = await _dbContext
                .Set<RefreshToken<TUser, TKey>>()
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Value == refreshTokenValue 
                                          && x.UserId.Equals(userId)
                                          && x.ExpiresIn > DateTime.UtcNow);

            ThrowExceptionIfTokenIsNull(token);

            token.Expire();
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TokenModel> FindActiveRefreshTokenAsync(TKey userId)
        {
            var activeRefreshToken = await _dbContext
                .Set<RefreshToken<TUser, TKey>>()
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.IsActive);

            ThrowExceptionIfTokenIsNull(activeRefreshToken);

            return BuildTokenModelByRefreshToken(activeRefreshToken);
        }

        public async Task<bool> IsPotentialRefreshTokenTheft(TKey userId, Guid refreshTokenValue)
        {
            var token = await FindRefreshToken(refreshTokenValue, userId);

            return (DateTime.UtcNow - token.ExpiredAt).Minutes > _options.AlreadyExpiredRefreshTokenTtlInMinutes;
        }

        private async Task<RefreshToken<TUser, TKey>> FindRefreshToken(Guid refreshTokenValue, TKey userId)
        {
            var token = await _dbContext
                .Set<RefreshToken<TUser, TKey>>()
                .Include(x => x.User)
                .AsQueryable()
                .Where(x => x.ExpiresIn > DateTime.UtcNow)
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefaultAsync(x => x.Value == refreshTokenValue);

            ThrowExceptionIfTokenIsNull(token);

            return token;
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

        private static TokenModel BuildTokenModelByRefreshToken(RefreshToken<TUser, TKey> refreshToken)
        {
            return new TokenModel
            {
                Value = refreshToken.Value.ToString(),
                ExpiresInUtc = refreshToken.ExpiresIn.ToUniversalTime(),
            };
        }

        private static void ThrowExceptionIfTokenIsNull(RefreshToken<TUser, TKey> token)
        {
            if (token == null)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenNotFound);
            }
        }

        private static void ThrowExceptionIfUserIsNull(RefreshToken<TUser, TKey> token)
        {
            if (token.User == null)
            {
                throw new AuthenticationException(ErrorTypes.UserNotFound);
            }
        }
    }
}