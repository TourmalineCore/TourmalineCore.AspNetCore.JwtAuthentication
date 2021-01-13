using System.Threading.Tasks;
using JwtAuthentication.Core.Models;

namespace JwtAuthentication.Core.Services
{
    internal interface ITokenManager
    {
        Task<TokenModel> GetAccessToken(string userName, string signingKey, int tokenLiveTime);
    }
}