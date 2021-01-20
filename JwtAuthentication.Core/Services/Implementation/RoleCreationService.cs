using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation
{
    internal class RoleCreationService : IRoleCreationService
    {
        private static string _route = "/roles/create";

        private readonly CustomRoleManager _customRoleManager;

        public RoleCreationService(CustomRoleManager customRoleManager)
        {
            _customRoleManager = customRoleManager;
        }

        public async Task<string> CreateRoleAsync(CreateRoleRequestModel model)
        {
            var role = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
            };

            await _customRoleManager.CreateAsync(role);

            return role.Id;
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