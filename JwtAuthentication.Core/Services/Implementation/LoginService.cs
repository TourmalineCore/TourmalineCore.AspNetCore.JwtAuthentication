using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class LoginService : ILoginService
    {
        private readonly ITokenManager _tokenManager;

        private readonly IUserCredentialsValidator _userCredentialsValidator;


        public LoginService(
            ITokenManager tokenManager,
            IUserCredentialsValidator userCredentialsValidator = null)
        {
            _tokenManager = tokenManager;
            _userCredentialsValidator = userCredentialsValidator;
        }

        public virtual async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            await ValidateCredentials(model);

            return new AuthResponseModel
            {
                AccessToken = await _tokenManager.GenerateAccessTokenAsync(model.Login)
            };
        }

        protected virtual async Task ValidateCredentials(LoginRequestModel model)
        {
            var isUserCredentialsValid = await _userCredentialsValidator.ValidateUserCredentials(model.Login, model.Password);

            if (!isUserCredentialsValid)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }
        }
    }
}