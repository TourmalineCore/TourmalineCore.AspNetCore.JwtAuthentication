using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh
{
    internal class RefreshMiddleware : RequestMiddlewareBase<IRefreshService, RefreshTokenRequestModel, AuthResponseModel>
    {
        private readonly RefreshEndpointOptions _endpointOptions;
        private readonly ILogger<RefreshMiddleware> _logger;

        private readonly Func<RefreshModel, Task> _onRefreshExecuting;
        private readonly Func<RefreshModel, Task> _onRefreshExecuted;

        public RefreshMiddleware(
            RequestDelegate next,
            RefreshEndpointOptions endpointOptions,
            ILogger<RefreshMiddleware> logger,
            Func<RefreshModel, Task> onRefreshExecuting,
            Func<RefreshModel, Task> onRefreshExecuted)
            : base(next)
        {
            _endpointOptions = endpointOptions;
            _logger = logger;
            _onRefreshExecuting = onRefreshExecuting;
            _onRefreshExecuted = onRefreshExecuted;
        }

        public async Task InvokeAsync(HttpContext context, IRefreshService refreshService)
        {
            await InvokeAsyncBase(context, refreshService, _endpointOptions.RefreshEndpointRoute);
        }

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(
            RefreshTokenRequestModel requestModel,
            IRefreshService service,
            HttpContext context)
        {
            var result = new AuthResponseModel();

            try
            {
                var contractRefreshModel = new RefreshModel
                {
                    RefreshTokenValue = requestModel.RefreshTokenValue,
                    ClientFingerPrint = requestModel.ClientFingerPrint,
                };

                await _onRefreshExecuting(contractRefreshModel);
                result = await service.RefreshAsync(contractRefreshModel.RefreshTokenValue, contractRefreshModel.ClientFingerPrint);
                await _onRefreshExecuted(contractRefreshModel);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                _logger.LogError(ex.ToString());
            }

            return result;
        }
    }
}