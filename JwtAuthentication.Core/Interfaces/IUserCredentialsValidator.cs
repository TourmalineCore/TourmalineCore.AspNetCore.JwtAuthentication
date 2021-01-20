using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Interfaces
{
    public interface IUserCredentialsValidator
    {
        Task<bool> ValidateUserCredentials(string login, string password);
    }
}