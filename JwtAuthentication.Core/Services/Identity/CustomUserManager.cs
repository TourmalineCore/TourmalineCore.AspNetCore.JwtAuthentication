using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity
{
    internal class CustomUserManager : UserManager<User>
    {
        public CustomUserManager(
            IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(store,
                    optionsAccessor,
                    passwordHasher,
                    userValidators,
                    passwordValidators,
                    keyNormalizer,
                    errors,
                    services,
                    logger
                )
        {
        }
    }
}