using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRegistrationService : IAuthService
    {
        public Task<long> RegistrationAsync(RegistrationRequestModel model);
    }
}