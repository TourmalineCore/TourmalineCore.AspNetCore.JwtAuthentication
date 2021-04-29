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
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshSignInManager<TUser> : SignInManager<TUser> where TUser : IdentityUser
    {
        private readonly IRefreshTokenManager _refreshTokenManager;
        private readonly ITokenManager _accessTokenManager;
        private readonly JwtAuthIdentityRefreshTokenDbContext<TUser> _dbContext;
        private readonly RefreshAuthenticationOptions _options;

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
            IOptions<RefreshAuthenticationOptions> options,
            JwtAuthIdentityRefreshTokenDbContext<TUser> dbContext)
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
            _options = options.Value;
        }

        public async Task<SignInResult> PasswordSignInAsync(TUser appUser, string password)
        {
            return await PasswordSignInAsync(appUser,
                    password,
                    false,
                    true
                );
        }

        public async Task<AuthResponseModel> GenerateAuthTokens(TUser appUser, string fingerPrint)
        {
            return new AuthResponseModel
            {
                AccessToken = await GetBearerToken(appUser, _options.SigningKey, _options.AccessTokenExpireInMinutes),
                RefreshToken = await _refreshTokenManager.GetRefreshToken(appUser, fingerPrint),
            };
        }

        public async Task<TokenModel> GetBearerToken(TUser appUser, string signingKey, int tokenLiveTime)
        {
            return await _accessTokenManager.GetAccessToken(appUser.NormalizedUserName, signingKey, tokenLiveTime);
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

            if (token != null)
            {
                token.IsActive = false;
                await _dbContext.SaveChangesAsync();
            }

            return token?.User;
        }

        public override Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            return Task.CompletedTask;
        }
    }
}