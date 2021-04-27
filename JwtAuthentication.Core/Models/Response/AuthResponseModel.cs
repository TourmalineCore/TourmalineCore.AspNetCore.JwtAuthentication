using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]
namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response
{
    internal class AuthResponseModel
    {
        public TokenModel AccessToken { get; set; }

        public TokenModel RefreshToken { get; set; }
    }
}