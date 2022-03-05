using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLogoutService<TUser> : ILogoutService where TUser : IdentityUser
    {
        private readonly IRefreshService _refreshLoginService;

        public IdentityLogoutService(IRefreshService refreshLoginService)
        {
            _refreshLoginService = refreshLoginService;
        }

        public async Task LogoutAsync(LogoutRequestModel model)
        {
            await _refreshLoginService.RefreshAsync(model.RefreshTokenValue, model.ClientFingerPrint);
        }
    }
}