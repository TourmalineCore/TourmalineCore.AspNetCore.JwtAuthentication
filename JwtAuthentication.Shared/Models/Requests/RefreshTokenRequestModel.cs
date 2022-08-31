using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests
{
    public class RefreshTokenRequestModel
    {
        public Guid RefreshTokenValue { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}