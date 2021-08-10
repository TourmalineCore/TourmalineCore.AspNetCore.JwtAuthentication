using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.Models
{
    public class RefreshModel
    {
        public Guid RefreshTokenValue { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}