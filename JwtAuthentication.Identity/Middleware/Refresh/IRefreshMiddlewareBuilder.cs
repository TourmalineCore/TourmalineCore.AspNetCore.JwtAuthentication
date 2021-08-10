using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh
{
    public interface IRefreshMiddlewareBuilder
    {
        public IRefreshMiddlewareBuilder OnRefreshExecuting(Func<RefreshModel, Task> callback);

        public IRefreshMiddlewareBuilder OnRefreshExecuted(Func<RefreshModel, Task> callback);

        public IApplicationBuilder UseRefreshMiddleware(RefreshEndpointOptions refreshEndpointOptions = null);
    }
}