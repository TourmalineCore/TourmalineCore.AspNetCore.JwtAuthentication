using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface IRefreshTokenManager<TUser, in TKey>
        where TUser : class    
        where TKey : IEquatable<TKey>
    {
        Task<BaseTokenModel> GenerateRefreshTokenAsync(object user, string clientFingerPrint);

        Task<TUser> GetRefreshTokenUserAsync(Guid refreshTokenValue, string clientFingerPrint);

        Task InvalidateRefreshTokenAsync(TKey userId, Guid refreshTokenValue);

        Task<bool> IsTokenActiveAsync(TKey userId, Guid refreshTokenValue);

        Task<bool> IsRefreshTokenInConfidenceIntervalAsync(TKey userId, Guid refreshTokenValue, int refreshConfidenceIntervalInMilliseconds);
    }
}