using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services
{
    internal class IdentityLoginService<TUser> : IdentityLoginService<TUser, string> where TUser : IdentityUser
    {
        public IdentityLoginService(ITokenManager tokenManager, SignInManager<TUser> signInManager)
            : base(tokenManager, signInManager)
        {
        }
    }

    internal class IdentityLoginService<TUser, TKey> : ILoginService where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly ITokenManager _tokenManager;

        private readonly SignInManager<TUser> _signInManager;

        public IdentityLoginService(
            ITokenManager tokenManager,
            SignInManager<TUser> signInManager)
        {
            _tokenManager = tokenManager;
            _signInManager = signInManager;
        }

        public async Task<AuthResponseModel> LoginAsync(LoginRequestModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Login);

            if (user is null)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

            var passwordIsCorrect = await _signInManager.UserManager.CheckPasswordAsync(user, model.Password);

            if (passwordIsCorrect == false)
            {
                throw new AuthenticationException(ErrorTypes.IncorrectLoginOrPassword);
            }

            var token = await _tokenManager.GetAccessToken(
                    model.Login
                );

            return new AuthResponseModel
            {
                AccessToken = new TokenModel
                {
                    Value = token.Value,
                    ExpiresInUtc = token.ExpiresInUtc,
                },
            };
        }
    }

}