using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityRegistrationService<TUser, TRegistrationRequestModel> : IdentityRegistrationService<TUser, string, TRegistrationRequestModel>
        where TUser : IdentityUser
        where TRegistrationRequestModel : RegistrationRequestModel
    {
        public IdentityRegistrationService(ILoginService loginService, SignInManager<TUser> signInManager)
            : base(loginService, signInManager)
        {
        }
    }

    internal class IdentityRegistrationService<TUser, TKey, TRegistrationRequestModel> : IRegistrationService<TUser, TRegistrationRequestModel>
        where TUser : IdentityUser<TKey>
        where TRegistrationRequestModel : RegistrationRequestModel
        where TKey : IEquatable<TKey>
    {
        private readonly ILoginService _loginService;

        private readonly SignInManager<TUser> _signInManager;

        public IdentityRegistrationService(
            ILoginService loginService,
            SignInManager<TUser> signInManager)
        {
            _loginService = loginService;
            _signInManager = signInManager;
        }

        public async Task<AuthResponseModel> RegisterAsync(TRegistrationRequestModel model, Func<TRegistrationRequestModel, TUser> mapping)
        {
            var user = mapping(model);
            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded == false)
            {
                throw new RegistrationException();
            }

            return await _loginService.LoginAsync(new LoginRequestModel
                    {
                        Login = model.Login,
                        Password = model.Password,
                    }
                );
        }
    }
}