using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts
{
    public interface IJwtTokenValidator
    {
        Task ValidateTokenAsync(string tokenValue);
        Task ValidateTokenTypeAsync(string tokenValue, string tokenType);
    }
}
