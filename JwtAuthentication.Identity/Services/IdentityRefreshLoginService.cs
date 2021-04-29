using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityRefreshLoginService<TUser> : ILoginService where TUser : IdentityUser
    {
        private readonly RefreshSignInManager<TUser> _signInManager;
        private readonly IValidator<RefreshTokenRequestModel> _refreshTokenValidator;

        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser> signInManager,
            IValidator<RefreshTokenRequestModel> refreshTokenValidator)
        {
            _signInManager = signInManager;
            _refreshTokenValidator = refreshTokenValidator;
        }

        public async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Login);

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password);

            
            if (signInResult.Succeeded == false)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

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