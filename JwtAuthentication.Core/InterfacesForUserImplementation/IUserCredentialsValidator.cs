using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.InterfacesForUserImplementation
{
    public interface IUserCredentialsValidator
    {
        public Task<bool> ValidateUserCredentials(string login, string password);
    }
}