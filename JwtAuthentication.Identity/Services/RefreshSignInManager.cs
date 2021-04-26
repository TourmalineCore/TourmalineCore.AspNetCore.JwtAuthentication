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
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class RefreshSignInManager<TUser> : SignInManager<TUser> where TUser : IdentityUser
    {
        private readonly RefreshTokenManager<TUser> _tokenManager;
        private readonly JwtAuthIdentityRefreshTokenDbContext<TUser> _dbContext;
        private readonly AuthenticationWithRefreshOptions _options;

        public RefreshSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger, 
            IAuthenticationSchemeProvider schemes, 
            IUserConfirmation<TUser> confirmation,
            RefreshTokenManager<TUser> tokenManager,
            IOptions<AuthenticationWithRefreshOptions> options,
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
            _tokenManager = tokenManager;
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

        public async Task<RefreshAuthResponseModel> GenerateAuthTokens(TUser appUser, string fingerPrint)
        {
            var accessToken = await GetBearerToken(appUser, _options.SigningKey, _options.AccessTokenExpireInMinutes);
            var refreshToken = _tokenManager.GetRefreshToken(appUser, fingerPrint);

            _dbContext.Attach(refreshToken.User);
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new RefreshAuthResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = new TokenModel
                {
                    Value = refreshToken.Value.ToString(), 
                    ExpiresInUtc = refreshToken.ExpiresIn.ToUniversalTime(),
                },
            };
        }

        public async Task<TokenModel> GetBearerToken(TUser appUser, string signingKey, int tokenLiveTime)
        {
            return await _tokenManager.GetAccessToken(appUser.NormalizedUserName, signingKey, tokenLiveTime);
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