using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout
{
    public class LogoutMiddlewareBuilder : ILogoutMiddlewareBuilder
    {
        private static Func<LogoutModel, Task> _onLogoutExecutingCallback = s => Task.CompletedTask;
        private static Func<LogoutModel, Task> _onLogoutExecutedCallback = s => Task.CompletedTask;

        private IApplicationBuilder _applicationBuilder;

        private static LogoutMiddlewareBuilder _instance;

        private LogoutMiddlewareBuilder()
        {
        }

        internal static LogoutMiddlewareBuilder GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new LogoutMiddlewareBuilder();

            return _instance;
        }

        internal ILogoutMiddlewareBuilder SetAppBuilder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the logout starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ILogoutMiddlewareBuilder OnLogoutExecuting(Func<LogoutModel, Task> callback)
        {
            _onLogoutExecutingCallback = callback;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when the logout ends.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ILogoutMiddlewareBuilder OnLogoutExecuted(Func<LogoutModel, Task> callback)
        {
            _onLogoutExecutedCallback = callback;
            return this;
        }

        /// <summary>
        /// Adds middleware to handle incoming logout requests.
        /// </summary>
        /// <param name="logoutEndpointOptions"></param>
        /// <returns></returns>
        public IApplicationBuilder UseLogoutMiddleware(LogoutEndpointOptions logoutEndpointOptions = null)
        {
            return _applicationBuilder
                .UseMiddleware<LogoutMiddleware>(logoutEndpointOptions ?? new LogoutEndpointOptions(), _onLogoutExecutingCallback, _onLogoutExecutedCallback);
        }
    }
}