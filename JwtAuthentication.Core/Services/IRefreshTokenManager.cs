using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager<TKey>
    {
        Task<TokenModel> GenerateRefreshToken(object user, string clientFingerPrint);

        Task InvalidateRefreshToken(TKey userId, Guid refreshTokenValue);

        Task<bool> IsTokenAlreadyInvalidated(TKey userId, Guid refreshTokenValue);

        Task<bool> IsPotentialRefreshTokenTheft(TKey userId, Guid refreshTokenValue);

        Task<TokenModel> FindActiveRefreshTokenAsync(TKey userId);
    }
}