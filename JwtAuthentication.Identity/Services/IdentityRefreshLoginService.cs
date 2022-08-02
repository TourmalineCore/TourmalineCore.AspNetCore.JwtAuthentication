using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

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
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
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

            var user = await _refreshTokenManager.FindRefreshTokenUserAsync(refreshTokenValue, clientFingerPrint);
            var userId = user.Id;

            if (TourmalineContextConfiguration.UseRefreshConfidenceInterval)
            {
                var isTokenAlreadyInvalidated = await _refreshTokenManager.IsTokenAlreadyInvalidatedAsync(userId, refreshTokenValue);

                if (!isTokenAlreadyInvalidated)
                {
                    await _refreshTokenManager.InvalidateRefreshTokenAsync(userId, refreshTokenValue);
                    return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
                }

                var isRefreshTokenStolen = await _refreshTokenManager.IsRefreshTokenStolenAsync(userId, refreshTokenValue, _refreshOptions.RefreshConfidenceIntervalInMilliseconds);

                if (isRefreshTokenStolen)
                {
                    throw new AuthenticationException(ErrorTypes.RefreshTokenIsStolen);
                }

                return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
            }

            await _refreshTokenManager.InvalidateRefreshTokenAsync(user.Id, refreshTokenValue);
            return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
        }
    }
}