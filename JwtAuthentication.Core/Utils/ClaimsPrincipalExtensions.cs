using System;
using System.Linq;
using System.Security.Claims;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Utils
{
    internal static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permissionName)
        {
            return claimsPrincipal.Claims
                .Any(x => x.Value.Equals(permissionName, StringComparison.InvariantCultureIgnoreCase)
                    );
        }
    }
}