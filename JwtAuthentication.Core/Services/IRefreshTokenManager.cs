using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager<TUser, in TKey>
        where TUser : class    
        where TKey : IEquatable<TKey>
    {
        Task<TokenModel> GenerateRefreshTokenAsync(object user, string clientFingerPrint);

        Task<TUser> GetRefreshTokenUserAsync(Guid refreshTokenValue, string clientFingerPrint);

        Task InvalidateRefreshTokenAsync(TKey userId, Guid refreshTokenValue);

        Task<bool> IsTokenActiveAsync(TKey userId, Guid refreshTokenValue);

        Task<bool> IsRefreshTokenInConfidenceIntervalAsync(TKey userId, Guid refreshTokenValue, int refreshConfidenceIntervalInMilliseconds);
    }
}