using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Registration.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Registration
{
    public class RegistrationMiddlewareBuilder : IRegistrationMiddlewareBuilder
    {
        private static Func<RegistrationModel, Task> _onRegistrationExecutingCallback = s => Task.CompletedTask;
        private static Func<RegistrationModel, Task> _onRegistrationExecutedCallback = s => Task.CompletedTask;

        private IApplicationBuilder _applicationBuilder;

        private static RegistrationMiddlewareBuilder _instance;

        private RegistrationMiddlewareBuilder()
        {
        }

        internal static RegistrationMiddlewareBuilder GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new RegistrationMiddlewareBuilder();

            return _instance;
        }

        internal IRegistrationMiddlewareBuilder SetAppBuilder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the registration starts.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IRegistrationMiddlewareBuilder OnRegistrationExecuting(Func<RegistrationModel, Task> callback)
        {
            _onRegistrationExecutingCallback = callback;
            return this;
        }

        /// <summary>
        /// Registering a callback function to perform actions when the registration ends.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IRegistrationMiddlewareBuilder OnRegistrationExecuted(Func<RegistrationModel, Task> callback)
        {
            _onRegistrationExecutedCallback = callback;
            return this;
        }

        /// <summary>
        /// Adds middleware to handle incoming user registration requests with custom registration request model. It requires a
        /// function to map model received from client.
        /// to user entity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRegistrationRequestModel"></typeparam>
        /// <param name="mapping"></param>
        /// <param name="registrationEndpointOptions"></param>
        /// <returns></returns>
        public IApplicationBuilder UseRegistration<TUser, TRegistrationRequestModel>(
            Func<TRegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser
            where TRegistrationRequestModel : RegistrationRequestModel
        {
            var options = registrationEndpointOptions ?? new RegistrationEndpointOptions();

            return _applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, TRegistrationRequestModel>>(mapping,
                        _onRegistrationExecutingCallback,
                        _onRegistrationExecutedCallback,
                        options
                    );
        }

        public IApplicationBuilder UseRegistration<TUser>(
            Func<RegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser
        {
            var options = registrationEndpointOptions ?? new RegistrationEndpointOptions();

            return _applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, RegistrationRequestModel>>(mapping,
                        _onRegistrationExecutingCallback,
                        _onRegistrationExecutedCallback,
                        options
                    );
        }
    }
}