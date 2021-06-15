using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class LogoutMiddleware : RequestMiddlewareBase<ILogoutService, LogoutRequestModel, bool>
    {
        private readonly LogoutEndpointOptions _endpointOptions;

        public LogoutMiddleware(RequestDelegate next, LogoutEndpointOptions endpointOptions)
            : base(next)
        {
            _endpointOptions = endpointOptions;
        }

        public async Task InvokeAsync(HttpContext context, ILogoutService logoutService)
        {
            await InvokeAsyncBase(context, logoutService, _endpointOptions.LogoutEndpointRoute);
        }

        protected override async Task<bool> ExecuteServiceMethod(
            LogoutRequestModel model,
            ILogoutService service,
            HttpContext context)
        {
            try
            {
                await service.LogoutAsync(model);
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return false;
            }

            return true;
        }
    }
}