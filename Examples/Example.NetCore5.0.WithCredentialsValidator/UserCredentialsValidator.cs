using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

namespace Example.NetCore.AuthenticationWithOwnCredentialsValidation
{
    public class UserCredentialsValidator : IUserCredentialsValidator
    {
        private const string Login = "Admin";
        private const string Password = "Admin";

        public Task<bool> ValidateUserCredentials(string login, string password)
        {
            return Task.FromResult(login == Login && password == Password);
        }
    }
}