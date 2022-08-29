using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login
{
    internal class DefaultLoginMiddlewareBuilder
    {
        private static Func<BasicLoginModel, Task> _onLoginExecutingCallback = null;
        private static Func<BasicLoginModel, Task> _onLoginExecutedCallback = null;
        private static BaseLoginEndpointOptions _loginEndpointOptions = new BaseLoginEndpointOptions();

        private static bool _loginMiddlewareIsRegistered = false;
        private static bool _executingCallbackIsRegistered = false;
        private static bool _executedCallbackIsRegistered = false;

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

        internal DefaultLoginMiddlewareBuilder SetAppBuilder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            return this;
        }

        internal DefaultLoginMiddlewareBuilder SetLoginEndpointOptions(BaseLoginEndpointOptions loginEndpointOptions = null)
        {
            if (loginEndpointOptions != null)
            {
                _loginEndpointOptions = loginEndpointOptions;
            }

            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the authentication starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IApplicationBuilder OnLoginExecuting(Func<BasicLoginModel, Task> callback)
        {
            _onLoginExecutingCallback = callback;
            _executingCallbackIsRegistered = true;

            if (!_loginMiddlewareIsRegistered)
            {
                return _applicationBuilder;
            }

            return _applicationBuilder.UseMiddleware<LoginMiddlewareWithExecutingCallback>(_loginEndpointOptions, _onLoginExecutingCallback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when the authentication ends.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IApplicationBuilder OnLoginExecuted(Func<BasicLoginModel, Task> callback)
        {
            _onLoginExecutedCallback = callback;
            _executedCallbackIsRegistered = true;

            if (!_loginMiddlewareIsRegistered)
            {
                return _applicationBuilder;
            }

            return _applicationBuilder.UseMiddleware<LoginMiddlewareWithExecutedCallback>(_loginEndpointOptions, _onLoginExecutedCallback);
        }

        /// <summary>
        /// Adds middleware to handle incoming login requests.
        /// </summary>
        /// <param name="loginEndpointOptions"></param>
        /// <returns></returns>
        public IApplicationBuilder UseDefaultLoginMiddleware()
        {            
            RegisterDefaultLoginMiddleware();
            return _applicationBuilder;
        }

        private void RegisterDefaultLoginMiddleware()
        {
            var middlewareArguments = new List<object>() { _loginEndpointOptions };

            if (_executingCallbackIsRegistered)
            {
                middlewareArguments.Add(_onLoginExecutingCallback);
            }

            if (_executedCallbackIsRegistered)
            {
                middlewareArguments.Add(_onLoginExecutedCallback);
            }

            _applicationBuilder.UseMiddleware<LoginMiddleware>(middlewareArguments.ToArray());
            _loginMiddlewareIsRegistered = true;
        }
    }
}