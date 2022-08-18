using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling
{
    public class IncorrectLoginOrPasswordException : Exception
    {
        public IncorrectLoginOrPasswordException() : base("Incorrect login or password") { }
    }
}
