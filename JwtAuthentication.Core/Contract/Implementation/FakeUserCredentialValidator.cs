using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract.Implementation
{
    internal class FakeUserCredentialValidator : IUserCredentialsValidator
    {
        private const string Login = "Admin";
        private const string Password = "Admin";

        public Task<bool> ValidateUserCredentials(string login, string password)
        {
            return Task.FromResult(login == Login && password == Password);
        }
    }
}