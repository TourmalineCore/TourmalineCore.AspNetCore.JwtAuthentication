using System.Threading.Tasks;
using JwtAuthentication.Core.Models.Request;
using JwtAuthentication.Core.Models.Response;

namespace JwtAuthentication.Core.Services
{
    internal interface ILoginService : IAuthService
    {
        public Task<AuthResponseModel> LoginAsync(LoginRequestModel model);
    }
}