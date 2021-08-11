using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models
{
    public class LogoutModel
    {
        public Guid RefreshTokenValue { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}