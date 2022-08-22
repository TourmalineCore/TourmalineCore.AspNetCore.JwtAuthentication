using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface ICoreRefreshService
    {
        public Task<TokenModel> RefreshAsync(string jwtRefreshToken);
    }
}