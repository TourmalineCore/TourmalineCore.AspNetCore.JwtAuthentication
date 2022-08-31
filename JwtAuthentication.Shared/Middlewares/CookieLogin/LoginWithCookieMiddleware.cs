using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.CookieLogin
{
    internal class LoginWithCookieMiddleware : RequestMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        private readonly ICookieAuthOptions _cookieOptions;
        private readonly ILoginEndpointOptions _loginEndpointOptions;

        public LoginWithCookieMiddleware(
            RequestDelegate next,
            ICookieAuthOptions cookieOptions,
            ILoginEndpointOptions loginEndpointOptions)
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
                            }
                        );
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                throw new Exception(ex.Message);
            }

            return new AuthResponseModel();
        }
    }
}