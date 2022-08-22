using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors
{
    public class RefreshTokenOrFingerprintNotFoundException : Exception
    {
        public RefreshTokenOrFingerprintNotFoundException() : base("Refresh token or fingerprint not found") { }
    }
}
