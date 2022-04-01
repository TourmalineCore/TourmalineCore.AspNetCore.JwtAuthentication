using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRefreshTokenManager<TUser, in TKey>
        where TUser : class    
        where TKey : IEquatable<TKey>
    {
        Task<TokenModel> GenerateRefreshToken(object user, string clientFingerPrint);

        Task<TUser> FindRefreshTokenUser(Guid refreshTokenValue, string clientFingerPrint);

        Task InvalidateRefreshToken(TKey userId, Guid refreshTokenValue);

        Task<bool> IsTokenAlreadyInvalidated(TKey userId, Guid refreshTokenValue);

        Task<bool> IsRefreshTokenStolen(TKey userId, Guid refreshTokenValue, int refreshConfidenceIntervalInSeconds);
    }
}