using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Signing;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices
{
    public class JwtTokenCreator : IJwtTokenCreator
    {
        private readonly AuthenticationOptions _authenticationOptions;

        public JwtTokenCreator(AuthenticationOptions authenticationOptions)
        {
            _authenticationOptions = authenticationOptions;
        }

        public async Task<TokenModel> CreateAsync(string tokenType, int tokenLifetimeInMinutes, List<Claim> claims = null)
        {
            if (!TokenType.IsAvailableTokenType(tokenType))
            {
                throw new ArgumentException("Invalid token type value");
            }

            if (tokenLifetimeInMinutes <= 0)
            {
                throw new ArgumentException("Token lifetime cannot be negative or zero");
            }

            if (claims == null)
            {
                claims = new List<Claim>();
            }

            AddTokenTypeClaim(claims, tokenType);

            var tokenExpiresIn = DateTime.UtcNow.AddMinutes(tokenLifetimeInMinutes);

            var token = new JwtSecurityToken(
                _authenticationOptions.Issuer,
                _authenticationOptions.Audience,
                claims,
                expires: tokenExpiresIn,
                signingCredentials: GetCredentials());

            return await Task.FromResult(new TokenModel
            {
                Value = new JwtSecurityTokenHandler().WriteToken(token),
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
