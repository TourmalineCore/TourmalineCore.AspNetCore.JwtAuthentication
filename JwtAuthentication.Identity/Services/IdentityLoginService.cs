using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLoginService<TUser> : ILoginService where TUser : IdentityUser
    {
        private readonly ITokenManager _tokenManager;

        private readonly AuthenticationOptions _options;
        private readonly SignInManager<TUser> _signInManager;

        public IdentityLoginService(
            ITokenManager tokenManager,
            SignInManager<TUser> signInManager,
            IOptions<AuthenticationOptions> options = null)
        {
            _tokenManager = tokenManager;
            _signInManager = signInManager;
            _options = options?.Value;
        }

        public async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Login);

            if (user is null)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

            var passwordIsCorrect = await _signInManager.UserManager.CheckPasswordAsync(user, model.Password);

            if (passwordIsCorrect == false)
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