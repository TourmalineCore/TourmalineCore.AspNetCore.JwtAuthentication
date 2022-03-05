using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLogoutService<TUser> : ILogoutService where TUser : IdentityUser
    {
        private readonly IdentityRefreshLoginService<TUser> _refreshService;

        public IdentityLogoutService(IdentityRefreshLoginService<TUser> refreshService)
        {
            _refreshService = refreshService;
        }

        public async Task LogoutAsync(string userName, LogoutRequestModel model)
        {
            await _refreshService.LogoutAsync(userName, model);
        }
    }
}