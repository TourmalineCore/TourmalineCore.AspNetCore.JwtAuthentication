using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("JwtAuthentication.Identity")]
namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response
{
    internal class AuthResponseModel
    {
        public TokenModel AccessToken { get; set; }
    }
}