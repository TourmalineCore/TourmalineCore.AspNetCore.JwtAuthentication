using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class RefreshService : ICoreRefreshService
    {
        private readonly ITokenManager _tokenManager;
        private readonly IJwtTokenValidator _jwtTokenValidator;

        public RefreshService(
            ITokenManager tokenManager,
            IJwtTokenValidator jwtTokenValidator)
        {
            _tokenManager = tokenManager;
            _jwtTokenValidator = jwtTokenValidator;
        }

        public async Task<TokenModel> RefreshAsync(string jwtRefreshToken)
        {
            try
            {
                _jwtTokenValidator.Validate(jwtRefreshToken);

                return await _tokenManager.GenerateAccessTokenAsync();
            } 
            catch (Exception)
            {
                throw new AuthenticationException(ErrorTypes.InvalidJwtToken);
            }
        }
    }
}
