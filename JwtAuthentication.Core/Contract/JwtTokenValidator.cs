using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Signing;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    internal class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public JwtTokenValidator(AuthenticationOptions options)
        {
            _authenticationOptions = options;
        }

        public Task ValidateTokenAsync(string tokenValue)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningHelper.GetPublicKey(_authenticationOptions.PublicSigningKey),
                ClockSkew = TimeSpan.Zero,
            };

            return Task.Run(() => handler.ValidateToken(tokenValue, tokenValidationParameters, out var _));
        }

        public Task ValidateTokenTypeAsync(string tokenValue, string tokenType)
        {
            if (!TokenType.IsAvailableTokenType(tokenType))
            {
                throw new ArgumentException("Invalid token type value");
            }

            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
            var tokenTypeClaim = token.Claims.SingleOrDefault(claim => claim.Type == Consts.TokenTypeClaimName);

            if (tokenTypeClaim == null)
            {
                throw new Exception("Token type claim is not set");
            }

            if (tokenTypeClaim.Value != tokenType)
            {
                throw new ArgumentException($"Token type is not '{tokenType}'");
            }

            return Task.CompletedTask;
        }
    }
}
