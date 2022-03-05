using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager<TUser>
    {
        Task<TokenModel> GenerateRefreshToken(object user, string clientFingerPrint);

        Task InvalidateRefreshToken(string userId, Guid refreshTokenValue);

        Task<bool> IsTokenAlreadyInvalidated(string userId, Guid refreshTokenValue);

        Task<bool> IsPotentialRefreshTokenTheft(string userId, Guid refreshTokenValue);

        Task<TokenModel> FindActiveRefreshTokenAsync(string userId);
    }
}