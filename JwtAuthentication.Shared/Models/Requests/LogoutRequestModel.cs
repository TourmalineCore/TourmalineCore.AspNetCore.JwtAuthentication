using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests
{
    public class LogoutRequestModel
    {
        public Guid RefreshTokenValue { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}