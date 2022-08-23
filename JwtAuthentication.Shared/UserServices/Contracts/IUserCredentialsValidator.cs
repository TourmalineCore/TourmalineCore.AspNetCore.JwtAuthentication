using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts
{
    public interface IUserCredentialsValidator
    {
        Task<bool> ValidateUserCredentials(string login, string password);
    }
}