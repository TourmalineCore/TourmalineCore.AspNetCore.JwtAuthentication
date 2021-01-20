using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Interfaces;

namespace Example.NetCore.PermissionBasedAuthorization
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        public Task<List<Claim>> GetUserClaimsAsync(string username)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim("Permission", "CanUseExample"),
                    }
                );
        }
    }
}