using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests
{
    public class CoreRefreshTokenRequest
    {
        public string RefreshTokenValue { get; set; }
    }
}