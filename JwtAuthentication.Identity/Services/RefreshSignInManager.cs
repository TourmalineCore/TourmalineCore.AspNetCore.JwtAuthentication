using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshSignInManager<TUser> : SignInManager<TUser> where TUser : IdentityUser
    {
        private readonly IRefreshTokenManager _refreshTokenManager;
        private readonly ITokenManager _accessTokenManager;
        private readonly TourmalineDbContext<TUser> _dbContext;

        public RefreshSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation,
            IRefreshTokenManager refreshTokenManager,
            ITokenManager accessTokenManager,
            TourmalineDbContext<TUser> dbContext)
            : base(userManager,
                    contextAccessor,
                    claimsFactory,
                    optionsAccessor,
                    logger,
                    schemes,
                    confirmation
                )
        {
            _refreshTokenManager = refreshTokenManager;
            _accessTokenManager = accessTokenManager;
            _dbContext = dbContext;
        }

        public async Task<AuthResponseModel> GenerateAuthTokens(TUser appUser, string fingerPrint)
        {
            return new AuthResponseModel
            {
                AccessToken = await GetBearerToken(appUser),
                RefreshToken = await _refreshTokenManager.GetRefreshToken(appUser, fingerPrint),
            };
        }

        private async Task<TokenModel> GetBearerToken(TUser appUser)
        {
            return await _accessTokenManager.GetAccessToken(appUser.NormalizedUserName);
        }

        public async Task<TUser> InvalidateRefreshTokenForUser(Guid refreshTokenValue, string fingerPrint = null)
        {
            var token = await _dbContext
                .Set<RefreshToken<TUser>>()
                .AsQueryable()
                .Include(x => x.User)
                .Where(x => x.IsActive)
                .Where(x => x.ExpiresIn > DateTime.UtcNow)
                .Where(x => fingerPrint == null || x.ClientFingerPrint == fingerPrint)
                .FirstOrDefaultAsync(x => x.Value == refreshTokenValue);

            if (token == null)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenOrFingerprintNotFound);
            }

            token.IsActive = false;
            await _dbContext.SaveChangesAsync();

            return token?.User;
        }

        public override Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            return Task.CompletedTask;
        }
    }

    internal class RefreshSignInManager<TUser, TKey> : SignInManager<TUser> where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IRefreshTokenManager _refreshTokenManager;
        private readonly ITokenManager _accessTokenManager;
        private readonly TourmalineDbContext<TUser, TKey> _dbContext;

        public RefreshSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation,
            IRefreshTokenManager refreshTokenManager,
            ITokenManager accessTokenManager,
            TourmalineDbContext<TUser, TKey> dbContext)
            : base(userManager,
                    contextAccessor,
                    claimsFactory,
                    optionsAccessor,
                    logger,
                    schemes,
                    confirmation
                )
        {
            _refreshTokenManager = refreshTokenManager;
            _accessTokenManager = accessTokenManager;
            _dbContext = dbContext;
        }

        public async Task<AuthResponseModel> GenerateAuthTokens(TUser appUser, string fingerPrint)
        {
            return new AuthResponseModel
            {
                AccessToken = await GetBearerToken(appUser),
                RefreshToken = await _refreshTokenManager.GetRefreshToken(appUser, fingerPrint),
            };
        }

        private async Task<TokenModel> GetBearerToken(TUser appUser)
        {
            return await _accessTokenManager.GetAccessToken(appUser.NormalizedUserName);
        }

        public async Task<TUser> InvalidateRefreshTokenForUser(Guid refreshTokenValue, string fingerPrint = null)
        {
            var token = await _dbContext
                .Set<RefreshToken<TUser, TKey>>()
                .AsQueryable()
                .Include(x => x.User)
                .Where(x => x.IsActive)
                .Where(x => x.ExpiresIn > DateTime.UtcNow)
                .Where(x => fingerPrint == null || x.ClientFingerPrint == fingerPrint)
                .FirstOrDefaultAsync(x => x.Value == refreshTokenValue);

            if (token == null)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenOrFingerprintNotFound);
            }

            token.IsActive = false;
            await _dbContext.SaveChangesAsync();

            return token?.User;
        }

        public override Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            return Task.CompletedTask;
        }
    }
}