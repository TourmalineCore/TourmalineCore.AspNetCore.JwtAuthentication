using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class TokenManager : ITokenManager
    {
        private readonly AuthenticationOptions _options;
        private readonly IUserClaimsProvider _userClaimsProvider;

        public TokenManager(
            IOptions<AuthenticationOptions> options,
            IUserClaimsProvider userClaimsProvider)
        {
            _options = options.Value;
            _userClaimsProvider = userClaimsProvider;
        }

        public async Task<TokenModel> GetAccessToken(string userName, string signingKey, int tokenLiveTime)
        {
            var claims = await _userClaimsProvider.GetUserClaimsAsync(userName);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(tokenLiveTime);

            var token = new JwtSecurityToken(_options.Issuer,
                    _options.Issuer,
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