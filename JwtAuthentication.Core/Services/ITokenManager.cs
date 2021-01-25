using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface ITokenManager
    {
        Task<TokenModel> GetAccessToken(string login, string signingKey, int tokenLiveTime);
    }
}