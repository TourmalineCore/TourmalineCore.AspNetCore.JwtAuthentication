using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
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

        public void Validate(string jwtTokenValue)
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

            handler.ValidateToken(jwtTokenValue, tokenValidationParameters, out var _);
        }
    }
}
