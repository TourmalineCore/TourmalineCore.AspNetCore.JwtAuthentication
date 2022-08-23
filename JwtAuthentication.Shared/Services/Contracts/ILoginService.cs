using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface ILoginService
    {
        public Task<AuthResponseModel> LoginAsync(LoginRequestModel model);
    }
}