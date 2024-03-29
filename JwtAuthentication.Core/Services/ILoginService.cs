using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface ILoginService
    {
        public Task<AuthResponseModel> LoginAsync(LoginRequestModel model);
    }
}