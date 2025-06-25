using System.Security.Claims;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

namespace Example.NetCore6._0.PermissionBasedAuthorization
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        public const string ExampleClaimType = "ExamplePermission";

        public const string FirstExampleClaimName = "CanUseExampleFirst";

        public const string SecondExampleClaimName = "CanUseExampleSecond";

        public Task<List<Claim>> GetUserClaimsAsync(string login)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim(ExampleClaimType, FirstExampleClaimName),
                        new Claim(ExampleClaimType, SecondExampleClaimName),
                    }
                );
        }
    }
}