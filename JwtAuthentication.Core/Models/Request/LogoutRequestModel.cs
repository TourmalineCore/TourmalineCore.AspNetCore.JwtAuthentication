using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]
namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request
{
    internal class LogoutRequestModel
    {
        public Guid RefreshTokenValue { get; set; }
        public string ClientFingerPrint { get; set; }
    }
}