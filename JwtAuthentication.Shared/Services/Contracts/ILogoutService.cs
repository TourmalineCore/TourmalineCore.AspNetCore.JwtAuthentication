using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface ILogoutService
    {
        public Task LogoutAsync(LogoutRequestModel model);
    }
}