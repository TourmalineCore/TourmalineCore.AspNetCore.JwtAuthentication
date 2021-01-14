using System.Threading.Tasks;
using JwtAuthentication.Core.ErrorHandling;
using JwtAuthentication.Core.InterfacesForUserImplementation;
using JwtAuthentication.Core.Models;
using JwtAuthentication.Core.Models.Request;
using JwtAuthentication.Core.Models.Response;
using JwtAuthentication.Core.Options;
using Microsoft.Extensions.Options;

namespace JwtAuthentication.Core.Services.Implementation
{
    internal class LoginService : ILoginService
    {
        private static string _route = "/auth/login";

        private readonly ITokenManager _tokenManager;

        private readonly IUserCredentialsValidator _userCredentialsValidator;

        private readonly AuthenticationOptions _options;

        public LoginService(
            ITokenManager tokenManager,
            IUserCredentialsValidator userCredentialsValidator = null,
            IOptions<AuthenticationOptions> options = null)
        {
            _tokenManager = tokenManager;
            _userCredentialsValidator = userCredentialsValidator;
            _options = options?.Value;
        }

        public virtual async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            var isUserCredentialsValid = await _userCredentialsValidator.ValidateUserCredentials(model.Login, model.Password);

            if (!isUserCredentialsValid)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

            var token = await _tokenManager.GetAccessToken(
                    model.Login,
                    _options.SigningKey,
                    _options.AccessTokenExpireInMinutes
                );

            return new AuthResponseModel
            {
                AccessToken = new TokenModel
                {
                    Value = token.Value,
                    ExpiresInUtc = token.ExpiresInUtc,
                },
            };
        }

        public string GetRoute()
        {
            return _route;
        }

        public static void OverrideRoute(string newRoute)
        {
            _route = newRoute;
        }
    }
}