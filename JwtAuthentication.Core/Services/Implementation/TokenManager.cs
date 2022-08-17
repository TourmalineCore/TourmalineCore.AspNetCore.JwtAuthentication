using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class TokenManager : ITokenManager
    {
        private readonly AuthenticationOptions _options;
        private readonly IUserClaimsProvider _userClaimsProvider;
        private readonly IJwtTokenCreator _jwtTokenCreator;

        public TokenManager(
            AuthenticationOptions options,
            IUserClaimsProvider userClaimsProvider,
            IJwtTokenCreator jwtTokenCreator)
        {
            _options = options;
            _userClaimsProvider = userClaimsProvider;
            _jwtTokenCreator = jwtTokenCreator;
        }

        public async Task<TokenModel> GenerateAccessTokenAsync(string login = null)
        {
            var claims = login == null
                ? new List<Claim>()
                : await _userClaimsProvider.GetUserClaimsAsync(login);

            return await _jwtTokenCreator.CreateAsync(TokenType.Access, _options.AccessTokenExpireInMinutes, claims);
        }
    }
}