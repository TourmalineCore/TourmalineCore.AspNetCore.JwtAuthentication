using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class LoginService : ILoginService
    {
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

        public async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            var isUserCredentialsValid = await _userCredentialsValidator.ValidateUserCredentials(model.Login, model.Password);

            if (!isUserCredentialsValid)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

            var token = await _tokenManager.GetAccessToken(
                    model.Login,
                    _options.PrivateSigningKey,
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
    }
}