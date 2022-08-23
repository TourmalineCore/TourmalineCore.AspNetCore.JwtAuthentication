using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services
{
    public class LoginService : ILoginService
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
                throw new IncorrectLoginOrPasswordException();
            }
        }
    }
}