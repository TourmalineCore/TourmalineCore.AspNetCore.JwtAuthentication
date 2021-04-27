using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshTokenManager<TUser> : TokenManager where TUser : IdentityUser
    {
        private readonly JwtAuthIdentityRefreshTokenDbContext<TUser> _dbContext;
        private readonly RefreshAuthenticationOptions _options;

        public RefreshTokenManager(
            JwtAuthIdentityRefreshTokenDbContext<TUser> dbContext,
            IOptions<RefreshAuthenticationOptions> options,
            IUserClaimsProvider userClaimsProvider) : base(options, userClaimsProvider)
        {
            _dbContext = dbContext;
            _options = options.Value;
        }

        public override async Task<TokenModel> GetRefreshToken(object user, string clientFingerPrint)
        {
            var refreshToken = CreateRefreshToken(user, clientFingerPrint);

            _dbContext.Attach(refreshToken.User);
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new TokenModel
            {
                Value = refreshToken.Value.ToString(),
                ExpiresInUtc = refreshToken.ExpiresIn.ToUniversalTime(),
            };
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
    }
}