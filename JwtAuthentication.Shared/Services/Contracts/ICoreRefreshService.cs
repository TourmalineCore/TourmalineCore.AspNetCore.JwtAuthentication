using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface ICoreRefreshService
    {
        public Task<TokenModel> RefreshAsync(string jwtRefreshToken);
    }
}
