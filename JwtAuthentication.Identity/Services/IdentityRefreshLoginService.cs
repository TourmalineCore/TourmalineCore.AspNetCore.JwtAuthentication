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
    internal class IdentityRefreshLoginService<TUser> : IdentityRefreshLoginService<TUser, string> where TUser : IdentityUser
    {
        public IdentityRefreshLoginService(RefreshSignInManager<TUser, string> signInManager, IValidator<RefreshTokenRequestModel> refreshTokenValidator, IUserCredentialsValidator userCredentialsValidator)
            : base(signInManager, refreshTokenValidator, userCredentialsValidator)
        {
        }
    }

    internal class IdentityRefreshLoginService<TUser, TKey> : ILoginService, IRefreshService where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly RefreshSignInManager<TUser, TKey> _signInManager;
        private readonly IValidator<RefreshTokenRequestModel> _refreshTokenValidator;
        private readonly IUserCredentialsValidator _userCredentialsValidator;

        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser, TKey> signInManager,
            IValidator<RefreshTokenRequestModel> refreshTokenValidator,
            IUserCredentialsValidator userCredentialsValidator)
        {
            _signInManager = signInManager;
            _refreshTokenValidator = refreshTokenValidator;
            _userCredentialsValidator = userCredentialsValidator;
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

        public async Task<AuthResponseModel> RefreshAsync(RefreshTokenRequestModel model)
        {
            await _refreshTokenValidator.ValidateAsync(model);

            try
            {
                var user = await _signInManager.InvalidateRefreshTokenForUser(model.RefreshTokenValue, model.ClientFingerPrint);

                return await _signInManager.GenerateAuthTokens(user, model.ClientFingerPrint);
            }
            catch (RefreshTokenException)
            {
                return await _signInManager.GetActiveRefreshToken(model.ClientFingerPrint);
            }
        }
    }
}