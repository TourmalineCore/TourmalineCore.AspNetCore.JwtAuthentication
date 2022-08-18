using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Refresh
{
    internal class RefreshMiddleware : RequestMiddlewareBase<ICoreRefreshService, CoreRefreshTokenRequestModel, TokenModel>
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

        public async Task InvokeAsync(HttpContext context, ICoreRefreshService refreshService)
        {
            await InvokeAsyncBase(context, refreshService, _endpointOptions.RefreshEndpointRoute);
        }

        protected override async Task<TokenModel> ExecuteServiceMethod(
            CoreRefreshTokenRequestModel requestModel,
            ICoreRefreshService service,
            HttpContext context)
        {
            var result = new TokenModel();

            try
            {
                var contractRefreshModel = new RefreshModel
                {
                    RefreshTokenValue = requestModel.RefreshTokenValue,
                };

                await _onRefreshExecuting(contractRefreshModel);
                result = await service.RefreshAsync(contractRefreshModel.RefreshTokenValue);
                await _onRefreshExecuted(contractRefreshModel);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                _logger.LogError(ex.ToString());
                throw new Exception(ex.Message);      
            }

            return result;
        }
    }
}