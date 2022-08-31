using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contracts;

namespace Example.NetCore3._1.PermissionBasedAuthentication
{
    public class CustomUserClaimsProvider : UserClaimsProvider
    {
        public const string ExampleClaimType = "ExamplePermission";

        public const string FirstExampleClaimName = "CanUseExampleFirst";

        public const string SecondExampleClaimName = "CanUseExampleSecond";

        public override Task<List<Claim>> GetUserClaimsAsync(string login)
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