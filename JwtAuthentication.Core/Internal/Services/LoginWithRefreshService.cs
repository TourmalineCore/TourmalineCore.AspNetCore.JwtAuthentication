using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Internal.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Internal.Services
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
