using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models
{
    internal class RefreshLoginRequestModel : LoginRequestModel
    {
        public string ClientFingerPrint { get; set; }
    }
}