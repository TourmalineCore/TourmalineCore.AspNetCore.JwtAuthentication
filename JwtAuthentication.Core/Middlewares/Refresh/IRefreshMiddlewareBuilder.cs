using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Refresh
{
    public interface IRefreshMiddlewareBuilder
    {
        public IRefreshMiddlewareBuilder OnRefreshExecuting(Func<RefreshModel, Task> callback);

        public IRefreshMiddlewareBuilder OnRefreshExecuted(Func<RefreshModel, Task> callback);

        public IApplicationBuilder UseRefreshMiddleware(RefreshEndpointOptions refreshEndpointOptions = null);
    }
}