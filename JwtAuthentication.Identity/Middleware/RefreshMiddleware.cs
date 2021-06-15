using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class RefreshMiddleware : RequestMiddlewareBase<IRefreshService, RefreshTokenRequestModel, AuthResponseModel>
    {
        private readonly RefreshEndpointOptions _endpointOptions;

        public RefreshMiddleware(RequestDelegate next, RefreshEndpointOptions endpointOptions)
            : base(next)
        {
            _endpointOptions = endpointOptions;
        }

        public async Task InvokeAsync(HttpContext context, IRefreshService refreshService)
        {
            await InvokeAsyncBase(context, refreshService, _endpointOptions.RefreshEndpointRoute);
        }

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(
            RefreshTokenRequestModel model,
            IRefreshService service,
            HttpContext context)
        {
            var result = new AuthResponseModel();

            try
            {
                result = await service.RefreshAsync(model);
            }
            catch (AuthenticationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            return result;
        }
    }
}