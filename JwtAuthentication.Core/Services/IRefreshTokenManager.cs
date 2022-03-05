using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager<TUser>
    {
        Task<TokenModel> GenerateRefreshToken(object user, string clientFingerPrint);

        Task<(TUser, bool tokenAlreadyInvalidated)> InvalidateRefreshToken(Guid refreshTokenValue, string clientFingerPrint);

        Task<TokenModel> FindActiveRefreshTokenAsync(string userId);

        Task<bool> IsPotentialRefreshTokenTheft(Guid refreshTokenValue, string userId);
    }
}