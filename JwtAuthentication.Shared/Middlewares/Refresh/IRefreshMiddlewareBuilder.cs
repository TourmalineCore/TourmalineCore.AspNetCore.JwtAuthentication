using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Refresh
{
    public interface IRefreshMiddlewareBuilder
    {
        public IRefreshMiddlewareBuilder OnRefreshExecuting(Func<RefreshModel, Task> callback);

        public IRefreshMiddlewareBuilder OnRefreshExecuted(Func<RefreshModel, Task> callback);

        public IApplicationBuilder UseRefreshMiddleware(RefreshEndpointOptions refreshEndpointOptions = null);
    }
}