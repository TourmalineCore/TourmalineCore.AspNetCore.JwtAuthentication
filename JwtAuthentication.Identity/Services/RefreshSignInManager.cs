using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshSignInManager<TUser> : RefreshSignInManager<TUser, string> where TUser : IdentityUser
    {
        public RefreshSignInManager(UserManager<TUser> userManager,
                                    IHttpContextAccessor contextAccessor,
                                    IUserClaimsPrincipalFactory<TUser> claimsFactory,
                                    IOptions<IdentityOptions> optionsAccessor,
                                    ILogger<SignInManager<TUser>> logger,
                                    IAuthenticationSchemeProvider schemes,
                                    IUserConfirmation<TUser> confirmation,
                                    IRefreshTokenManager<TUser, string> refreshTokenManager,
                                    ITokenManager accessTokenManager)
            : base(userManager, contextAccessor, claimsFactory,
                    optionsAccessor,
                    logger,
                    schemes,
                    confirmation,
                    refreshTokenManager,
                    accessTokenManager
                )
        {
        }
    }

    internal class RefreshSignInManager<TUser, TKey> : SignInManager<TUser> 
        where TUser : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly IRefreshTokenManager<TUser, TKey> _refreshTokenManager;
        private readonly ITokenManager _accessTokenManager;

        public RefreshSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation,
            IRefreshTokenManager<TUser, TKey> refreshTokenManager,
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
            _accessTokenManager = accessTokenManager;
            _refreshTokenManager = refreshTokenManager;
        }

        public async Task<AuthResponseModel> GenerateAuthTokens(TUser appUser, string clientFingerPrint)
        {
            return new AuthResponseModel
            {
                AccessToken = await _accessTokenManager.GenerateAccessTokenAsync(appUser.NormalizedUserName),
                RefreshToken = await _refreshTokenManager.GenerateRefreshTokenAsync(appUser, clientFingerPrint),
            };
        }

        public override Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
        {
            return Task.CompletedTask;
        }
    }
}