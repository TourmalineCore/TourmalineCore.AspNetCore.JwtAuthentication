using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]
[assembly: InternalsVisibleTo("Tests")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses
{
    public class AuthResponseModel
    {
        public BaseTokenModel AccessToken { get; set; }

        public BaseTokenModel RefreshToken { get; set; }
    }
}