using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class RegistrationService : IRegistrationService
    {
        private static string _route = "/users/registration";

        private readonly CustomUserManager _customUserManager;

        public RegistrationService(CustomUserManager customUserManager)
        {
            _customUserManager = customUserManager;
        }

        public async Task<long> RegistrationAsync(RegistrationRequestModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true,
            };

            await _customUserManager.CreateAsync(user, model.Password);

            return user.Id;
        }

        public string GetRoute()
        {
            return _route;
        }

        public static void OverrideRoute(string newRoute)
        {
            _route = newRoute;
        }
    }
}