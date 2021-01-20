using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

namespace Example.NetCore.PermissionBasedAuthorization
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        public const string ExampleClaimType = "ExamplePermission";

        public const string ExampleClaimName = "CanUseExample";

        public Task<List<Claim>> GetUserClaimsAsync(string login)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim(ExampleClaimType, ExampleClaimName),
                    }
                );
        }
    }
}