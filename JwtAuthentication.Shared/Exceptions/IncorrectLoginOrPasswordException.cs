using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors
{
    public class IncorrectLoginOrPasswordException : Exception
    {
        public IncorrectLoginOrPasswordException() : base("Incorrect login or password") { }
    }
}
