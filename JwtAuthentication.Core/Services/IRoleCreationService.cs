using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRoleCreationService : IAuthService
    {
        public Task<string> CreateRoleAsync(CreateRoleRequestModel model);
    }
}