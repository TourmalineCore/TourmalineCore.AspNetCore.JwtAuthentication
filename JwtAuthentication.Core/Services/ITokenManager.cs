using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface ITokenManager
    {
        Task<TokenModel> GenerateAccessTokenAsync(string login = null);
    }
}