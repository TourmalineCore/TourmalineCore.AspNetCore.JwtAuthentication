using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class CoreRefreshTokenManager : ICoreRefreshTokenManager
    {
        private readonly IJwtTokenCreator _jwtTokenCreator;
        private readonly RefreshTokenOptions _refreshTokenOptions;

        public CoreRefreshTokenManager(IJwtTokenCreator jwtTokenCreator, RefreshTokenOptions refreshTokenOptions)
        {
            _jwtTokenCreator = jwtTokenCreator;
            _refreshTokenOptions = refreshTokenOptions;
        }

        public Task<TokenModel> GenerateRefreshTokenAsync()
        {          
           return _jwtTokenCreator.CreateAsync(TokenType.Refresh, _refreshTokenOptions.RefreshTokenExpireInMinutes);           
        }
    }
}
