using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity
{
    internal class CustomRoleManager : RoleManager<Role>
    {
        public CustomRoleManager(
            IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger)
            : base(store,
                    roleValidators,
                    keyNormalizer,
                    errors,
                    logger
                )
        {
        }
    }
}