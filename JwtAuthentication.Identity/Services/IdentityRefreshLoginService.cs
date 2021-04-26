using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityRefreshLoginService<TUser> : ILoginService where TUser : IdentityUser
    {
        private static string _route = "/auth/login";
        private static string _refreshTokenRoute = "/auth/refresh";

        private readonly AuthenticationWithRefreshOptions _options;
        private readonly RefreshSignInManager<TUser> _signInManager;
        private readonly IValidator<RefreshTokenRequestModel> _refreshTokenValidator;

        public IdentityRefreshLoginService(
            RefreshSignInManager<TUser> signInManager,
            IValidator<RefreshTokenRequestModel> refreshTokenValidator,
            IOptions<AuthenticationWithRefreshOptions> options = null)
        {
            _signInManager = signInManager;
            _refreshTokenValidator = refreshTokenValidator;
            _options = options?.Value;
        }

        public async Task<RefreshAuthResponseModel> LoginAsync(RefreshLoginRequestModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Login);

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password);

            
            if (signInResult.Succeeded == false)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

            return await _signInManager.GenerateAuthTokens(user, model.ClientFingerPrint);
        }

        public async Task<RefreshAuthResponseModel> RefreshTokenAsync(RefreshTokenRequestModel model)
        {
            await _refreshTokenValidator.ValidateAsync(model);

            var user = await _signInManager.InvalidateRefreshTokenForUser(model.RefreshTokenValue, model.ClientFingerPrint);

            if (user == null)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenOrFingerprintNotFound);
            }

            return await _signInManager.GenerateAuthTokens(user, model.ClientFingerPrint);
        }

        public string GetRoute()
        {
            return _route;
        }

        public string GetRefreshTokenRoute()
        {
            return _refreshTokenRoute;
        }

        public static void OverrideRoute(string newRoute)
        {
            _route = newRoute;
        }

        public static void OverrideRefreshTokenRoute(string newRoute)
        {
            _refreshTokenRoute = newRoute;
        }

        public Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}