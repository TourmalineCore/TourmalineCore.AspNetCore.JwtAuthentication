using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager
    {
        Task<TokenModel> GetRefreshToken(object user, string clientFingerPrint);
    }
}