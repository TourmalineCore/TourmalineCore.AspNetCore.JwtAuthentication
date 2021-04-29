using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class LoginWithCookieMiddleware : RequestMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {

        private readonly CookieAuthOptions _cookieOptions;
        private readonly LoginEndpointOptions _loginEndpointOptions;

        public LoginWithCookieMiddleware(
            RequestDelegate next, 
            CookieAuthOptions cookieOptions, 
            LoginEndpointOptions loginEndpointOptions)
            : base(next)
        {
            _cookieOptions = cookieOptions;
            _loginEndpointOptions = loginEndpointOptions;
        }

        public async Task InvokeAsync(HttpContext context, ILoginService loginService)
        {
            await InvokeAsyncBase(context, loginService, _loginEndpointOptions.LoginEndpointRoute);
        }

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(
            LoginRequestModel model,
            ILoginService service,
            HttpContext context)
        {
            try
            {
                var result = await service.LoginAsync(model);

                if (result.AccessToken != null)
                {
                    context.Response.Cookies.Append(
                        _cookieOptions.Key,
                        result.AccessToken.Value,
                        new CookieOptions
                        {
                            Expires = result.AccessToken.ExpiresInUtc,
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                        });
                }
            }
            catch (AuthenticationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            return new AuthResponseModel();
        }
    }
}