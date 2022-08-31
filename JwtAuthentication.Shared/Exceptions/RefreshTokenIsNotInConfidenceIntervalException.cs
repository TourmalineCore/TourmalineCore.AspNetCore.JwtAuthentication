using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors
{
    public class RefreshTokenIsNotInConfidenceIntervalException : Exception
    {
        public RefreshTokenIsNotInConfidenceIntervalException() : base("Refresh token is not in confidence interval") { }
    }
}
