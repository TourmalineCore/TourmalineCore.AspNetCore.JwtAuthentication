using System.Threading.Tasks;
using JwtAuthentication.Core.Models.Request;
using JwtAuthentication.Core.Models.Response;
using JwtAuthentication.Core.Services;
using Microsoft.AspNetCore.Http;

namespace JwtAuthentication.Core.Middlewares
{
    internal class LoginMiddleware : AuthMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        public LoginMiddleware(RequestDelegate next)
            : base(next)
        {

        }

        public async Task InvokeAsync(HttpContext context, ILoginService loginService)
        {
            await InvokeAsyncBase(context, loginService);
        }

        protected override Task<AuthResponseModel> ServiceMethodInvoke(LoginRequestModel model, ILoginService service)
        {
            return service.LoginAsync(model);
        }
    }
}