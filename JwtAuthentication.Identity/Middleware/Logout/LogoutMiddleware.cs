using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout
{
    internal class LogoutMiddleware : RequestMiddlewareBase<ILogoutService, LogoutRequestModel, bool>
    {
        private readonly LogoutEndpointOptions _endpointOptions;
        private readonly ILogger<LogoutMiddleware> _logger;

        private readonly Func<LogoutModel, Task> _onLogoutExecuting;
        private readonly Func<LogoutModel, Task> _onLogoutExecuted;

        public LogoutMiddleware(
            RequestDelegate next,
            LogoutEndpointOptions endpointOptions,
            ILogger<LogoutMiddleware> logger,
            Func<LogoutModel, Task> onLogoutExecuting, 
            Func<LogoutModel, Task> onLogoutExecuted)
            : base(next)
        {
            _endpointOptions = endpointOptions;
            _logger = logger;
            _onLogoutExecuting = onLogoutExecuting;
            _onLogoutExecuted = onLogoutExecuted;
        }

        public async Task InvokeAsync(HttpContext context, ILogoutService logoutService)
        {
            await InvokeAsyncBase(context, logoutService, _endpointOptions.LogoutEndpointRoute);
        }

        protected override async Task<bool> ExecuteServiceMethod(
            LogoutRequestModel requestModel,
            ILogoutService service,
            HttpContext context)
        {
            try
            {
                var contractLogoutModel = new LogoutModel
                {
                    RefreshTokenValue = requestModel.RefreshTokenValue,
                    ClientFingerPrint = requestModel.ClientFingerPrint,
                };

                await _onLogoutExecuting(contractLogoutModel);
                await service.LogoutAsync(requestModel);
                await _onLogoutExecuted(contractLogoutModel);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                _logger.LogError(ex.ToString());
                return false;
            }

            return true;
        }
    }
}