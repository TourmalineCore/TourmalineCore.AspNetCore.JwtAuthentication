using System;

namespace JwtAuthentication.Core.ErrorHandling
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(ErrorTypes errorType)
        {
            ExceptionInfo = errorType;
        }

        private ErrorTypes ExceptionInfo { get; }
    }
}