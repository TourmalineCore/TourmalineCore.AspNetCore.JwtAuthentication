using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(ErrorTypes errorType)
        {
            ExceptionInfo = errorType;
        }

        public ErrorTypes ExceptionInfo { get; }
    }
}