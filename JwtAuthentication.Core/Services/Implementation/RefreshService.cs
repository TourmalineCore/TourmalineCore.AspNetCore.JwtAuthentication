using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class RefreshService : ICoreRefreshService
    {
        private readonly IJwtTokenValidator _jwtTokenValidator;
        private readonly IJwtTokenCreator _jwtTokenCreator;
        private readonly RefreshTokenOptions _refreshTokenOptions;

        public RefreshService(
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

                return await _jwtTokenCreator.CreateAsync(TokenType.Refresh, new List<Claim>(), DateTime.UtcNow.AddMinutes(_refreshTokenOptions.RefreshTokenExpireInMinutes));
            } 
            catch (Exception)
            {
                throw new AuthenticationException(ErrorTypes.InvalidJwtToken);
            }
        }
    }
}
