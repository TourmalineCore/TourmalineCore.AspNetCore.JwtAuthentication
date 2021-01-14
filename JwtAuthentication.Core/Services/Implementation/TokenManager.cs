using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using JwtAuthentication.Core.Models;
using System.Threading.Tasks;
using JwtAuthentication.Core.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthentication.Core.Services.Implementation
{
    internal class TokenManager : ITokenManager
    {
        private readonly AuthenticationOptions _options;

        public TokenManager(IOptions<AuthenticationOptions> options)
        {
            _options = options.Value;
        }

        public async Task<TokenModel> GetAccessToken(string userName, string signingKey, int tokenLiveTime)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userName),
                }
                .ToList();

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