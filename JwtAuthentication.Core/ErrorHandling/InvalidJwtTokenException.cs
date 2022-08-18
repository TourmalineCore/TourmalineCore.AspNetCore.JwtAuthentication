using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public class InvalidJwtTokenException : Exception
    {
        public InvalidJwtTokenException() : base("Invalid jwt token") { }
    }
}
