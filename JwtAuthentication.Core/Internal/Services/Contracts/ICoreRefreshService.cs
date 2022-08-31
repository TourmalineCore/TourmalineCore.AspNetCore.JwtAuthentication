using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Internal.Services.Contracts
{
    internal interface ICoreRefreshService
    {
        public Task<BaseTokenModel> RefreshAsync(string jwtRefreshToken);
    }
}