using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts
{
    public interface IJwtTokenCreator
    {
        Task<TokenModel> CreateAsync(string tokenType, int tokenLifetimeInMinutes, List<Claim> claims = null);
    }
}
