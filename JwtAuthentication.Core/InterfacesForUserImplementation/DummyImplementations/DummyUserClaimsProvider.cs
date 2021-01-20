using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.InterfacesForUserImplementation.DummyImplementations
{
    public class DummyUserClaimsProvider : IUserClaimsProvider
    {
        public Task<List<Claim>> GetUserClaimsAsync(string username)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, username),
                    }
                );
        }
    }
}