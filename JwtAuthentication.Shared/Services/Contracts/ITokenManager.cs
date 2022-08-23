using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface ITokenManager
    {
        Task<BaseTokenModel> GenerateAccessTokenAsync(string login = null);
    }
}