using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Internal.Services.Contracts
{
    internal interface ICoreRefreshTokenManager
    {
        Task<BaseTokenModel> GenerateRefreshTokenAsync();
    }
}
