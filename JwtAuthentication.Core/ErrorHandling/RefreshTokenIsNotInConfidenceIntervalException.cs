using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public class RefreshTokenIsNotInConfidenceIntervalException : Exception
    {
        public RefreshTokenIsNotInConfidenceIntervalException() : base("Refresh token is not in confidence interval") { }
    }
}
