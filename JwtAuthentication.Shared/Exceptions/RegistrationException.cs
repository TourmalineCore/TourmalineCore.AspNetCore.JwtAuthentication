using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors
{
    public class RegistrationException : Exception
    {
        public RegistrationException() : base("An registration exception occured") { }
    }
}