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

        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser> signInManager,
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

            var user = await _signInManager.InvalidateRefreshTokenForUser(model.RefreshTokenValue, model.ClientFingerPrint);

            if (user == null)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenOrFingerprintNotFound);
            }

            return await _signInManager.GenerateAuthTokens(user, model.ClientFingerPrint);
        }
    }
}