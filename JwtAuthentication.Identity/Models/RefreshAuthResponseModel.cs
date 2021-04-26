using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models
{
    internal class RefreshAuthResponseModel : AuthResponseModel
    {
        public TokenModel RefreshToken { get; set; }
    }
}