using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    internal interface IJwtTokenValidator
    {
        Task ValidateTokenAsync(string tokenValue);
        Task ValidateTokenTypeAsync(string tokenValue, string tokenType);
    }
}
