using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityRefreshLoginService<TUser> : ILoginService, IRefreshService, ILogoutService where TUser : IdentityUser
    {
        private readonly RefreshSignInManager<TUser> _signInManager;
        private readonly IValidator<RefreshTokenRequestModel> _refreshTokenValidator;
        private readonly IUserCredentialsValidator _userCredentialsValidator;
        private readonly IRefreshTokenManager<TUser> _refreshTokenManager;

        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser> signInManager,
            IValidator<RefreshTokenRequestModel> refreshTokenValidator,
            IUserCredentialsValidator userCredentialsValidator,
            IRefreshTokenManager<TUser> refreshTokenManager)
        {
            _signInManager = signInManager;
            _refreshTokenValidator = refreshTokenValidator;
            _userCredentialsValidator = userCredentialsValidator;
            _refreshTokenManager = refreshTokenManager;
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

        public async Task<AuthResponseModel> RefreshAsync(string userName, Guid refreshTokenValue, string clientFingerPrint)
        {
            await _refreshTokenValidator.ValidateAsync(new RefreshTokenRequestModel
            {
                RefreshTokenValue = refreshTokenValue,
                ClientFingerPrint = clientFingerPrint,
            });

            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            var userId = user.Id;

            var isTokenAlreadyInvalidated = await _refreshTokenManager.IsTokenAlreadyInvalidated(userId, refreshTokenValue);

            if (!isTokenAlreadyInvalidated)
            {
                await _refreshTokenManager.InvalidateRefreshToken(userId, refreshTokenValue);
                return await _signInManager.GenerateAuthTokens(user, clientFingerPrint);
            }

            var isPotentialRefreshTokenTheft = await _refreshTokenManager.IsPotentialRefreshTokenTheft(userId, refreshTokenValue);

            if (isPotentialRefreshTokenTheft)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenIsProbablyStolen);
            }

            return await _signInManager.GenerateAuthTokens(user);
        }

        public async Task LogoutAsync(string userName, LogoutRequestModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            await _refreshTokenManager.InvalidateRefreshToken(user.Id, model.RefreshTokenValue);
        }
    }
}