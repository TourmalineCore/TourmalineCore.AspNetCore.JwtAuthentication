using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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

        public async Task<TokenModel> GetAccessToken(string login, string signingKey, int tokenLiveTime)
        {
            var claims = await _userClaimsProvider.GetUserClaimsAsync(login);
            var privateKey = SigningHelper.GetPrivateKey(signingKey);
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            var expires = DateTime.UtcNow.AddMinutes(tokenLiveTime);

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