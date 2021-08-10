using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login
{
    public class DefaultLoginMiddlewareBuilder : IDefaultLoginMiddlewareBuilder
    {
        private static Func<LoginModel, Task> _onLoginExecutingCallback = s => Task.CompletedTask;
        private static Func<LoginModel, Task> _onLoginExecutedCallback = s => Task.CompletedTask;

        private IApplicationBuilder _applicationBuilder;

        private static DefaultLoginMiddlewareBuilder _instance;

        private DefaultLoginMiddlewareBuilder()
        {
        }

        internal static DefaultLoginMiddlewareBuilder GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new DefaultLoginMiddlewareBuilder();

            return _instance;
        }

        internal IDefaultLoginMiddlewareBuilder SetAppBuilder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the authentication starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IDefaultLoginMiddlewareBuilder OnLoginExecuting(Func<LoginModel, Task> callback)
        {
            _onLoginExecutingCallback = callback;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when the authentication ends.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IDefaultLoginMiddlewareBuilder OnLoginExecuted(Func<LoginModel, Task> callback)
        {
            _onLoginExecutedCallback = callback;
            return this;
        }

        /// <summary>
        /// Adds middleware to handle incoming login requests.
        /// </summary>
        /// <param name="loginEndpointOptions"></param>
        /// <returns></returns>
        public IApplicationBuilder UseDefaultLoginMiddleware(LoginEndpointOptions loginEndpointOptions = null)
        {
            return _applicationBuilder
                .UseMiddleware<LoginMiddleware>(loginEndpointOptions ?? new LoginEndpointOptions(), _onLoginExecutingCallback, _onLoginExecutedCallback);
        }
    }
}