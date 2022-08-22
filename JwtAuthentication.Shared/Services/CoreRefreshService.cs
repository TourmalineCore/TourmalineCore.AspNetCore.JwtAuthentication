using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services
{
    public class CoreRefreshService : ICoreRefreshService
    {
        private readonly IJwtTokenValidator _jwtTokenValidator;
        private readonly IJwtTokenCreator _jwtTokenCreator;
        private readonly RefreshTokenOptions _refreshTokenOptions;

        public CoreRefreshService(
            IJwtTokenValidator jwtTokenValidator,
            IJwtTokenCreator jwtTokenCreator, 
            RefreshTokenOptions refreshTokenOptions)
        {
            _jwtTokenValidator = jwtTokenValidator;
            _jwtTokenCreator = jwtTokenCreator;
            _refreshTokenOptions = refreshTokenOptions;
        }

        public async Task<TokenModel> RefreshAsync(string tokenValue)
        {
            try
            {
                await _jwtTokenValidator.ValidateTokenAsync(tokenValue);
                await _jwtTokenValidator.ValidateTokenTypeAsync(tokenValue, TokenType.Refresh);

                return await _jwtTokenCreator.CreateAsync(TokenType.Refresh, _refreshTokenOptions.RefreshTokenExpireInMinutes);
            } 
            catch (Exception)
            {
                throw new InvalidJwtTokenException();
            }
        }
    }
}
