using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface ICoreRefreshTokenManager
    {
        Task<TokenModel> GenerateRefreshTokenAsync();
    }
}
