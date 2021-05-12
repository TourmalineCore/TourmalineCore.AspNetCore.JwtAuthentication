using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]
[assembly: InternalsVisibleTo("Tests")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response
{
    internal class AuthResponseModel
    {
        public TokenModel AccessToken { get; set; }

        public TokenModel RefreshToken { get; set; }
    }
}