using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators
{
    internal class IdentityUserCredentialsValidator<TUser> : IUserCredentialsValidator where TUser : IdentityUser
    {
        private readonly SignInManager<TUser> _signInManager;

        public IdentityUserCredentialsValidator(SignInManager<TUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> ValidateUserCredentials(string login, string password)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(login);

            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);

            return signInResult.Succeeded;
        }
    }
}
