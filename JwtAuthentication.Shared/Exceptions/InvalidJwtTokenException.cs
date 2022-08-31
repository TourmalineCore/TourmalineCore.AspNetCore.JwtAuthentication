using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors
{
    public class InvalidJwtTokenException : Exception
    {
        public InvalidJwtTokenException() : base("Invalid jwt token") { }
    }
}
