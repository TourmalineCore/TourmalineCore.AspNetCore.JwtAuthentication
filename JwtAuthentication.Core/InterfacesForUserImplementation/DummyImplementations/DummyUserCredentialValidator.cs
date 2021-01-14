using System.Threading.Tasks;

namespace JwtAuthentication.Core.InterfacesForUserImplementation.DummyImplementations
{
    public class DummyUserCredentialValidator : IUserCredentialsValidator
    {
        private const string Login = "Admin1";
        private const string Password = "Admin1";

        public Task<bool> ValidateUserCredentials(string login, string password)
        {
            return Task.FromResult(login == Login && password == Password);
        }
    }
}