using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contracts
{
    public abstract class UserCredentialsValidator : IUserCredentialsValidator
    {
        public abstract Task<bool> ValidateUserCredentials(string login, string password);
    }
}
