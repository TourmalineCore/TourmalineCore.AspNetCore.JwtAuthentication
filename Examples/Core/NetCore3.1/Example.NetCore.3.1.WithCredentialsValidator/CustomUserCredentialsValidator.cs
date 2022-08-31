using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contracts;

namespace Example.NetCore3._1.WithCredentialsValidator
{
    public class CustomUserCredentialsValidator : UserCredentialsValidator
    {
        private const string Login = "User";
        private const string Password = "User";

        public override Task<bool> ValidateUserCredentials(string login, string password)
        {
            return Task.FromResult(login == Login && password == Password);
        }
    }
}
