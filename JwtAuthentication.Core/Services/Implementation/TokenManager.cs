using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Signing;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class TokenManager : ITokenManager
    {
        private readonly AuthenticationOptions _options;
        private readonly IUserClaimsProvider _userClaimsProvider;

        public TokenManager(
            AuthenticationOptions options,
            IUserClaimsProvider userClaimsProvider)
        {
            _options = options;
            _userClaimsProvider = userClaimsProvider;
        }

        public async Task<TokenModel> GenerateAccessTokenAsync(string login = null)
        {
            var claims = login == null
                ? new List<Claim>()
                : await _userClaimsProvider.GetUserClaimsAsync(login);

            var privateKey = SigningHelper.GetPrivateKey(_options.PrivateSigningKey);
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpireInMinutes);

            var token = new JwtSecurityToken(_options.Issuer,
                    _options.Audience,
                    claims,
                    expires: expires,
                    signingCredentials: credentials
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(new TokenModel
                    {
                        Value = tokenValue,
                        ExpiresInUtc = expires.ToUniversalTime(),
                    }
                );
        }
    }
}