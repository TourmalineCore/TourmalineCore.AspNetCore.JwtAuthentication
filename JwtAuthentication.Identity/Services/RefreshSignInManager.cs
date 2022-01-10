using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshSignInManager<TUser> : SignInManager<TUser> where TUser : IdentityUser
    {
        private readonly IRefreshTokenManager<TUser> _refreshTokenManager;
        private readonly ITokenManager _accessTokenManager;

        public RefreshSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation,
            IRefreshTokenManager<TUser> refreshTokenManager,
            ITokenManager accessTokenManager)
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
        }

        public async Task<AuthResponseModel> GenerateAuthTokens(TUser appUser, string fingerPrint)
        {
            return new AuthResponseModel
            {
                AccessToken = await GetBearerToken(appUser),
                RefreshToken = await _refreshTokenManager.GetRefreshToken(appUser, fingerPrint),
            };
        }

        public async Task<AuthResponseModel> GetActiveRefreshToken(string clientFingerPrint)
        {
            var (appUser, refreshToken) = await _refreshTokenManager.FindActiveRefreshToken(clientFingerPrint);

            return new AuthResponseModel
            {
                AccessToken = await GetBearerToken(appUser),
                RefreshToken = refreshToken,
            };
        }

        public async Task<TUser> InvalidateRefreshTokenForUser(Guid refreshTokenValue, string clientFingerPrint = null)
        {
            return await _refreshTokenManager.InvalidateRefreshToken(refreshTokenValue, clientFingerPrint);
        }

        public override Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            return Task.CompletedTask;
        }

        private async Task<TokenModel> GetBearerToken(TUser appUser)
        {
            return await _accessTokenManager.GetAccessToken(appUser.NormalizedUserName);
        }
    }
}