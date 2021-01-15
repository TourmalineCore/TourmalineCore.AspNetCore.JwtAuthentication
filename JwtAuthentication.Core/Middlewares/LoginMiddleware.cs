using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
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

        protected override Task<AuthResponseModel> ExecuteServiceMethod(LoginRequestModel model, ILoginService service)
        {
            return service.LoginAsync(model);
        }
    }
}