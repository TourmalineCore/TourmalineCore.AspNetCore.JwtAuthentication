using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract.Implementation
{
    internal class DefaultUserClaimsProvider : IUserClaimsProvider
    {
        public Task<List<Claim>> GetUserClaimsAsync(string login)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, login),
                    }
                );
        }
    }
}