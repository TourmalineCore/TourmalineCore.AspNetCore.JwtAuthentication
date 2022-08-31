using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityRefreshLoginService<TUser> : IdentityRefreshLoginService<TUser, string> where TUser : IdentityUser
    {
        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser, string> signInManager, 
            IValidator<RefreshTokenRequestModel> refreshTokenValidator, 
            IUserCredentialsValidator userCredentialsValidator,
            IRefreshTokenManager<TUser, string> refreshTokenManager,
            RefreshOptions refreshOptions)
            : base(
                signInManager, 
                refreshTokenValidator, 
                userCredentialsValidator,
                refreshTokenManager,
                refreshOptions)
        {
        }
    }

    internal class IdentityRefreshLoginService<TUser, TKey> : ILoginService, IRefreshService
        where TUser : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly RefreshSignInManager<TUser, TKey> _signInManager;
        private readonly IValidator<RefreshTokenRequestModel> _refreshTokenValidator;
        private readonly IUserCredentialsValidator _userCredentialsValidator;
        private readonly IRefreshTokenManager<TUser, TKey> _refreshTokenManager;
        private readonly RefreshOptions _refreshOptions;

        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser, TKey> signInManager,
            IValidator<RefreshTokenRequestModel> refreshTokenValidator,
            IUserCredentialsValidator userCredentialsValidator,
            IRefreshTokenManager<TUser, TKey> refreshTokenManager, 
            RefreshOptions refreshOptions)
        {
            _signInManager = signInManager;
            _refreshTokenValidator = refreshTokenValidator;
            _userCredentialsValidator = userCredentialsValidator;
            _refreshTokenManager = refreshTokenManager;
            _refreshOptions = refreshOptions;
        }

        public async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            var signInResult = await _userCredentialsValidator.ValidateUserCredentials(model.Login, model.Password);

            if (signInResult == false)
            {
                throw new IncorrectLoginOrPasswordException();
            }

            var user = await _signInManager.UserManager.FindByNameAsync(model.Login);
            return await _signInManager.GenerateAuthTokens(user, model.ClientFingerPrint);
        }

        public async Task<AuthResponseModel> RefreshAsync(Guid refreshTokenValue, string clientFingerPrint)
        {
            await _refreshTokenValidator.ValidateAsync(new RefreshTokenRequestModel
            {
                RefreshTokenValue = refreshTokenValue,
                ClientFingerPrint = clientFingerPrint,
            });

            var user = await _refreshTokenManager.GetRefreshTokenUserAsync(refreshTokenValue, clientFingerPrint);

            if (TourmalineContextConfiguration.UseRefreshConfidenceInterval)
            {
                return await GenerateAuthTokensWhenRefreshConfidenceIntervalIsEnabledAsync(user, refreshTokenValue, clientFingerPrint);
            }

            await _refreshTokenManager.InvalidateRefreshTokenAsync(user.Id, refreshTokenValue);
            return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
        }

        private async Task<AuthResponseModel> GenerateAuthTokensWhenRefreshConfidenceIntervalIsEnabledAsync(TUser user, Guid refreshTokenValue, string clientFingerPrint)
        {
            var isTokenActive = await _refreshTokenManager.IsTokenActiveAsync(user.Id, refreshTokenValue);

            if (isTokenActive)
            {
                await _refreshTokenManager.InvalidateRefreshTokenAsync(user.Id, refreshTokenValue);
                return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
            }

            var refreshTokenIsInConfidenceInterval = await _refreshTokenManager.IsRefreshTokenInConfidenceIntervalAsync(user.Id, refreshTokenValue, _refreshOptions.RefreshConfidenceIntervalInMilliseconds);

            if (refreshTokenIsInConfidenceInterval)
            {
                return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
            }

            throw new RefreshTokenIsNotInConfidenceIntervalException();
        }
    }
}