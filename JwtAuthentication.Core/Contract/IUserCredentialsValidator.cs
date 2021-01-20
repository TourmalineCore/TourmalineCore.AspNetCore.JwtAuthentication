using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    public interface IUserCredentialsValidator
    {
        Task<bool> ValidateUserCredentials(string login, string password);
    }
}