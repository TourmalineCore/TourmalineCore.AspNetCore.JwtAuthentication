using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public class RefreshTokenException : Exception
    {
        public RefreshTokenException(ErrorTypes errorType)
        {
            ExceptionInfo = errorType;
        }

        private ErrorTypes ExceptionInfo { get; }
    }
}