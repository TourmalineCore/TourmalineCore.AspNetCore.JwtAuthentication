using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager<TUser>
    {
        Task<TokenModel> GetRefreshToken(object user, string clientFingerPrint);

        Task<TUser> InvalidateRefreshToken(Guid refreshTokenValue, string clientFingerPrint);

        Task<(TUser, TokenModel)> FindActiveRefreshToken(string clientFingerPrint);
    }
}