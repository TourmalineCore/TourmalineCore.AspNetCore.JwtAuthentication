using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public class RefreshTokenOrFingerprintNotFoundException : Exception
    {
        public RefreshTokenOrFingerprintNotFoundException() : base("Refresh token or fingerprint not found") { }
    }
}
