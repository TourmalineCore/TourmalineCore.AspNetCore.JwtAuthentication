using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request
{
    public class CoreRefreshTokenRequestModel
    {
        public string RefreshTokenValue { get; set; }
    }
}