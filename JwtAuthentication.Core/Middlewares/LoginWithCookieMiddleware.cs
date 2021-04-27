using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class LoginWithCookieMiddleware : AuthMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {

        private readonly CookieAuthOptions _options;

        public LoginWithCookieMiddleware(RequestDelegate next, CookieAuthOptions options)
            : base(next)
        {
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context, ILoginService loginService)
        {
            await InvokeAsyncBase(context, loginService);
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
                        _options.Key,
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