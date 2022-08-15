using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class LoginWithRefreshService : LoginService
    {
        private readonly ITokenManager _tokenManager;

        public LoginWithRefreshService(
            ITokenManager tokenManager,
            IUserCredentialsValidator userCredentialsValidator = null) 
            : base(tokenManager, userCredentialsValidator)
        {
            _tokenManager = tokenManager;
        }

        public override async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            await base.ValidateCredentials(model);            

            return new AuthResponseModel
            {
                AccessToken = await _tokenManager.GenerateAccessTokenAsync(model.Login),
                RefreshToken = await _tokenManager.GenerateAccessTokenAsync(model.Login),
            };
        }
    }
}
