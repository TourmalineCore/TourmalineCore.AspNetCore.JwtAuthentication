using System.Threading.Tasks;
using JwtAuthentication.Core.Models.Request;
using JwtAuthentication.Core.Models.Response;

namespace JwtAuthentication.Core.Services
{
    internal interface ILoginService
    {
        public Task<AuthResponseModel> LoginAsync(LoginRequestModel model);
    }
}