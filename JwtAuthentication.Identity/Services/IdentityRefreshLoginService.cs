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
    internal class IdentityRefreshLoginService<TUser> : ILoginService, IRefreshService where TUser : IdentityUser
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
            var refreshToken = await _refreshTokenManager.GenerateRefreshToken(user, model.ClientFingerPrint);
            return await _signInManager.GenerateAuthTokens(user, refreshToken);
        }

        public async Task<AuthResponseModel> RefreshAsync(Guid refreshTokenValue, string clientFingerPrint)
        {
            await _refreshTokenValidator.ValidateAsync(new RefreshTokenRequestModel
            {
                RefreshTokenValue = refreshTokenValue,
                ClientFingerPrint = clientFingerPrint,
            });

            var (appUser, tokenAlreadyInvalidated) = await _refreshTokenManager.InvalidateRefreshToken(refreshTokenValue, clientFingerPrint);

            if (!tokenAlreadyInvalidated)
            {
                var refreshToken = await _refreshTokenManager.GenerateRefreshToken(appUser, clientFingerPrint);
                return await _signInManager.GenerateAuthTokens(appUser, refreshToken);
            }

            if (await _refreshTokenManager.IsPotentialRefreshTokenTheft(refreshTokenValue, appUser.Id))
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenIsProbablyStolen);
            }

            var activeRefreshToken = await _refreshTokenManager.FindActiveRefreshTokenAsync(appUser.Id);

            return await _signInManager.GenerateAuthTokens(appUser, activeRefreshToken);
        }
    }
}