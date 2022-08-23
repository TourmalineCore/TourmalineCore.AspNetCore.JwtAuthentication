using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices
{
    public class FakeUserCredentialValidator : IUserCredentialsValidator
    {
        private const string Login = "Admin";
        private const string Password = "Admin";

        public Task<bool> ValidateUserCredentials(string login, string password)
        {
            return Task.FromResult(login == Login && password == Password);
        }
    }
}