using System;
using System.Linq;
using System.Security.Claims;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Utils
{
    internal static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permissionType, string permissionName)
        {
            return claimsPrincipal
                .Claims
                .Any(x => x.Type == permissionType
                          && x.Value.Equals(permissionName, StringComparison.InvariantCultureIgnoreCase)
                    );
        }
    }
}