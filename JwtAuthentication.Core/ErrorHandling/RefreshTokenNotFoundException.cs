using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    internal class RefreshTokenNotFoundException : Exception
    {
        public RefreshTokenNotFoundException() : base("Refresh token not found") { }
    }
}
