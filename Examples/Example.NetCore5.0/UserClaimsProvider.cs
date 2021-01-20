using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.InterfacesForUserImplementation;

namespace Example.NetCore5._0
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        public Task<List<Claim>> GetUserClaimsAsync(string username)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim("Permissions", "CanRun"),
                    }
                );
        }
    }
}