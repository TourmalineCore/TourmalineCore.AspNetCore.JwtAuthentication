using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators
{
    internal class IdentityUserCredentialsValidator<TUser> : IdentityUserCredentialsValidator<TUser, string> 
        where TUser : IdentityUser
    {
        public IdentityUserCredentialsValidator(SignInManager<TUser> signInManager)
            : base(signInManager)
        {
        }
    }

    internal class IdentityUserCredentialsValidator<TUser, TKey> : IUserCredentialsValidator 
        where TUser : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        private readonly SignInManager<TUser> _signInManager;

        public IdentityUserCredentialsValidator(SignInManager<TUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> ValidateUserCredentials(string login, string password)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(login);

            if (user == null)
            {
                return false;
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user,
                    password,
                    false,
                    true
                );

            return signInResult.Succeeded;
        }
    }
}