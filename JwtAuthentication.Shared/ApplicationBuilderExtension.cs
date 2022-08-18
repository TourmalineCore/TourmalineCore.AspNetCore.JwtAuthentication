using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Refresh;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared
{
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Adds middleware to handle incoming login and token refresh requests.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="endpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRefreshTokenMiddleware(this IApplicationBuilder applicationBuilder, RefreshEndpointOptions endpointOptions = null)
        {
            Func<RefreshModel, Task> defaultOnRefreshCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<RefreshMiddleware>(endpointOptions ?? new RefreshEndpointOptions(), defaultOnRefreshCallback, defaultOnRefreshCallback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the refresh starts.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IRefreshMiddlewareBuilder OnRefreshExecuting(this IApplicationBuilder applicationBuilder, Func<RefreshModel, Task> callback)
        {
            return RefreshMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnRefreshExecuting(callback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when the refresh ends.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IRefreshMiddlewareBuilder OnRefreshExecuted(this IApplicationBuilder applicationBuilder, Func<RefreshModel, Task> callback)
        {
            return RefreshMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnRefreshExecuted(callback);
        }
    }
}