using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IAuthenticationOptions _options;
        private readonly IUserClaimsProvider _userClaimsProvider;
        private readonly IJwtTokenCreator _jwtTokenCreator;

        public TokenManager(
            IAuthenticationOptions options,
            IUserClaimsProvider userClaimsProvider,
            IJwtTokenCreator jwtTokenCreator)
        {
            _options = options;
            _userClaimsProvider = userClaimsProvider;
            _jwtTokenCreator = jwtTokenCreator;
        }

        public async Task<BaseTokenModel> GenerateAccessTokenAsync(string login = null)
        {
            var claims = login == null
                ? new List<Claim>()
                : await _userClaimsProvider.GetUserClaimsAsync(login);

            return await _jwtTokenCreator.CreateAsync(TokenType.Access, _options.AccessTokenExpireInMinutes, claims);
        }
    }
}