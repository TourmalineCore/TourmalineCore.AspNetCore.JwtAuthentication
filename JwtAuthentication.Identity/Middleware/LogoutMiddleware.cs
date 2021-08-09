using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class LogoutMiddleware : RequestMiddlewareBase<ILogoutService, LogoutRequestModel, bool>
    {
        private readonly LogoutEndpointOptions _endpointOptions;
        private readonly ILogger<LogoutMiddleware> _logger;

        public LogoutMiddleware(RequestDelegate next, LogoutEndpointOptions endpointOptions, ILogger<LogoutMiddleware> logger)
            : base(next)
        {
            _endpointOptions = endpointOptions;
            _logger = logger;
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
            catch(AuthenticationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                _logger.LogError(ex.ToString());
                return false;
            }

            return true;
        }
    }
}