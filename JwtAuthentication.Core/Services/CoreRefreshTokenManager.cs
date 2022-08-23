using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services
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

        public Task<BaseTokenModel> GenerateRefreshTokenAsync()
        {          
           return _jwtTokenCreator.CreateAsync(TokenType.Refresh, _refreshTokenOptions.RefreshTokenExpireInMinutes);           
        }
    }
}
