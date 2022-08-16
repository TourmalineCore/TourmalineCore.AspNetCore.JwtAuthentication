using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Signing;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    internal class JwtTokenCreator : IJwtTokenCreator
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public JwtTokenCreator(AuthenticationOptions authenticationOptions)
        {
            _authenticationOptions = authenticationOptions;
        }

        public async Task<TokenModel> CreateAsync(string tokenType, List<Claim> claims, DateTime tokenExpiresIn)
        {
            if (!TokenType.IsAvailableTokenType(tokenType))
            {
                throw new ArgumentException("Invalid token type value");
            }

            AddTokenTypeClaim(claims, tokenType);

            var token = new JwtSecurityToken(
                _authenticationOptions.Issuer,
                _authenticationOptions.Audience,
                claims,
                expires: tokenExpiresIn,
                signingCredentials: GetCredentials());

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(new TokenModel
            {
                Value = tokenValue,
                ExpiresInUtc = tokenExpiresIn.ToUniversalTime(),
            });
        }

        private SigningCredentials GetCredentials()
        {
            var privateKey = SigningHelper.GetPrivateKey(_authenticationOptions.PrivateSigningKey);

            return new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
        }

        private void AddTokenTypeClaim(List<Claim> claims, string tokenTypeValue)
        {
            var isTokenTypeClaimSet = claims.Exists(x => x.Type == Consts.TokenTypeClaimName);

            if (!isTokenTypeClaimSet)
            {
                claims.Add(new Claim(Consts.TokenTypeClaimName, tokenTypeValue));
            }
        }
    }
}
