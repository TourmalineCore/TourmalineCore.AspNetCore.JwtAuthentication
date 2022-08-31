using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors
{
    public class RefreshTokenNotFoundException : Exception
    {
        public RefreshTokenNotFoundException() : base("Refresh token not found") { }
    }
}
