using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("JwtAuthentication.Identity")]
namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request
{
    internal class LoginRequestModel
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }
}