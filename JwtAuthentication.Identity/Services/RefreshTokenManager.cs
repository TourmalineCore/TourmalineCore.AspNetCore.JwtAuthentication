using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshTokenManager<TUser> : IRefreshTokenManager<TUser> where TUser : IdentityUser
    {
        private readonly TourmalineDbContext<TUser> _dbContext;
        private readonly RefreshAuthenticationOptions _options;

        public RefreshTokenManager(
            TourmalineDbContext<TUser> dbContext,
            RefreshAuthenticationOptions options)
        {
            _dbContext = dbContext;
            _options = options;
        }

        public async Task<TokenModel> GetRefreshToken(object user, string clientFingerPrint)
        {
            var refreshToken = CreateRefreshToken(user, clientFingerPrint);

            _dbContext.Attach(refreshToken.User);
            await _dbContext.Set<RefreshToken<TUser>>().AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return BuildTokenModelByRefreshToken(refreshToken);
        }

        public async Task<TUser> InvalidateRefreshToken(Guid refreshTokenValue, string clientFingerPrint)
        {
            var token = await _dbContext
                .Set<RefreshToken<TUser>>()
                .AsQueryable()
                .Include(x => x.User)
                .Where(x => x.ExpiresIn > DateTime.UtcNow)
                .Where(x => clientFingerPrint == null || x.ClientFingerPrint == clientFingerPrint)
                .FirstOrDefaultAsync(x => x.Value == refreshTokenValue);

            ThrowExceptionIfTokenOrUserIsNull(token);

            if (!token.IsActive)
            {
                throw new RefreshTokenException(ErrorTypes.RefreshTokenHasAlreadyBeenInvalidated);
            }

            token.IsActive = false;
            await _dbContext.SaveChangesAsync();

            return token.User;
        }

        public async Task<(TUser, TokenModel)> FindActiveRefreshToken(string clientFingerPrint)
        {
            if (clientFingerPrint == null)
            {
                throw new AuthenticationException(ErrorTypes.FingerprintCannotBeNull);
            }

            var activeRefreshToken = await _dbContext
                .Set<RefreshToken<TUser>>()
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ClientFingerPrint == clientFingerPrint);

            ThrowExceptionIfTokenOrUserIsNull(activeRefreshToken);

            return (activeRefreshToken.User, BuildTokenModelByRefreshToken(activeRefreshToken));
        }


        private RefreshToken<TUser> CreateRefreshToken(object user, string clientFingerPrint)
        {
            var expiresDate = DateTime.UtcNow.AddMinutes(_options.RefreshTokenExpireInMinutes);

            var newToken = new RefreshToken<TUser>
            {
                Value = Guid.NewGuid(),
                ExpiresIn = expiresDate,
                IsActive = true,
                ClientFingerPrint = clientFingerPrint,
                User = (TUser)user,
            };

            return newToken;
        }

        private static TokenModel BuildTokenModelByRefreshToken(RefreshToken<TUser> refreshToken)
        {
            return new TokenModel
            {
                Value = refreshToken.Value.ToString(),
                ExpiresInUtc = refreshToken.ExpiresIn.ToUniversalTime(),
            };
        }

        private static void ThrowExceptionIfTokenOrUserIsNull(RefreshToken<TUser> token)
        {
            if (token == null)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenNotFound);
            }

            if (token.User == null)
            {
                throw new AuthenticationException(ErrorTypes.UserNotFound);
            }
        }
    }
}