using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class LoginMiddleware : RequestMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        private readonly LoginEndpointOptions _loginEndpointOptions;

        public LoginMiddleware(RequestDelegate next, LoginEndpointOptions loginEndpointOptions)
            : base(next)
        {
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
            var result = new AuthResponseModel();

            try
            {
                result = await service.LoginAsync(model);
            }
            catch (AuthenticationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            return result;
        }
    }
}