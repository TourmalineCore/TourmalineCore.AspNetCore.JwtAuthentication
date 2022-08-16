using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    public class JwtTokenCreator : IJwtTokenCreator
    {
        public string Create(string issuer, string audience, List<Claim> claims, DateTime expires, SigningCredentials credentials, string tokenType)
        {
            if (!TokenType.IsTokenType(tokenType))
            {
                throw new ArgumentException("Invalid token type value");
            }

            AddTokenTypeClaim(claims, tokenType);

            var token = new JwtSecurityToken(issuer,
                    audience,
                    claims,
                    expires: expires,
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void AddTokenTypeClaim(List<Claim> claims, string tokenType)
        {
            var isTokenTypeClaimSet = claims.Exists(x => x.Type == Consts.TokenTypeClaimName);

            if (!isTokenTypeClaimSet)
            {
                claims.Add(new Claim(Consts.TokenTypeClaimName, tokenType));
            }
        }
    }
}
