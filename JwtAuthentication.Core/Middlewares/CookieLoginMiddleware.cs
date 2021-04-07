using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class CookieLoginMiddleware : AuthMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        public CookieLoginMiddleware(RequestDelegate next)
            : base(next)
        {
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
                        ".AspNetCore.Application.Id",
                        result.AccessToken.Value,
                        new CookieOptions
                        {
                            Expires = result.AccessToken.ExpiresInUtc,
                        });

                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    context.Response.Headers.Add("X-Xss-Protection", "1");
                    context.Response.Headers.Add("X-Frame-Options", "DENY");
                }
            }
            catch (AuthenticationException)
            {
                context.Response.StatusCode = 401;
            }

            return new AuthResponseModel();
        }
    }
}