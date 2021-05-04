using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request
{
    public class RefreshTokenRequestModel
    {
        public Guid RefreshTokenValue { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}