using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests
{
    public class LoginRequestModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}