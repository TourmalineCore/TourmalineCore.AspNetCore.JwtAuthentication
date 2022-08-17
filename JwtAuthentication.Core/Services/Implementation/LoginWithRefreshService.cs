using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class LoginWithRefreshService : LoginService
    {
        private readonly ITokenManager _tokenManager;
        private readonly ICoreRefreshTokenManager _refreshTokenManager;

        public LoginWithRefreshService(
            ITokenManager tokenManager,
            ICoreRefreshTokenManager refreshTokenManager,
            IUserCredentialsValidator userCredentialsValidator = null) 
            : base(tokenManager, userCredentialsValidator)
        {
            _tokenManager = tokenManager;
            _refreshTokenManager = refreshTokenManager;
        }

        public override async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            await base.ValidateCredentials(model);            

            return new AuthResponseModel
            {
                AccessToken = await _tokenManager.GenerateAccessTokenAsync(model.Login),
                RefreshToken = await _refreshTokenManager.GenerateRefreshTokenAsync(),
            };
        }
    }
}
