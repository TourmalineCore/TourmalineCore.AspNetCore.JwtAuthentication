using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Refresh
{
    public class RefreshMiddlewareBuilder : IRefreshMiddlewareBuilder
    {
        private static Func<RefreshModel, Task> _onRefreshExecutingCallback = s => Task.CompletedTask;
        private static Func<RefreshModel, Task> _onRefreshExecutedCallback = s => Task.CompletedTask;

        private IApplicationBuilder _applicationBuilder;

        private static RefreshMiddlewareBuilder _instance;

        private RefreshMiddlewareBuilder()
        {
        }

        internal static RefreshMiddlewareBuilder GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new RefreshMiddlewareBuilder();

            return _instance;
        }

        internal IRefreshMiddlewareBuilder SetAppBuilder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the refresh starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IRefreshMiddlewareBuilder OnRefreshExecuting(Func<RefreshModel, Task> callback)
        {
            _onRefreshExecutingCallback = callback;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when the refresh ends.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IRefreshMiddlewareBuilder OnRefreshExecuted(Func<RefreshModel, Task> callback)
        {
            _onRefreshExecutedCallback = callback;
            return this;
        }

        /// <summary>
        /// Adds middleware to handle incoming logout requests.
        /// </summary>
        /// <param name="refreshEndpointOptions"></param>
        /// <returns></returns>
        public IApplicationBuilder UseRefreshMiddleware(RefreshEndpointOptions refreshEndpointOptions = null)
        {
            return _applicationBuilder
                .UseMiddleware<RefreshMiddleware>(refreshEndpointOptions ?? new RefreshEndpointOptions(), _onRefreshExecutingCallback, _onRefreshExecutedCallback);
        }
    }
}