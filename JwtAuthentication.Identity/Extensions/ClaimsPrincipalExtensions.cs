using System.Security.Claims;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        private const string NameIdentifierClaimType = "nameIdentifier";

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            var phoneNumberClaim = claimsPrincipal.FindFirst(NameIdentifierClaimType);

            return phoneNumberClaim?.Value;
        }
    }
}