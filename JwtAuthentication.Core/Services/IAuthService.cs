using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("JwtAuthentication.Identity")]
namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IAuthService
    {
        public string GetRoute();
    }
}