using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models
{
    public class RefreshTokenRequestModel
    {
        public Guid RefreshTokenValue { get; set; }
        public string ClientFingerPrint { get; set; }
    }
}
