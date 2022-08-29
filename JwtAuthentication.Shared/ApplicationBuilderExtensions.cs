using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.CookieLogin;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds Authentication and Authorization to the app.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseAuthentication()
                .UseAuthorization();
        }

        /// <summary>
        /// Adds middleware to handle incoming login requests.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="loginEndpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultLoginMiddleware(this IApplicationBuilder applicationBuilder, ILoginEndpointOptions loginEndpointOptions)
        {
            return DefaultLoginMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .UseDefaultLoginMiddleware(loginEndpointOptions);
        }

        /// <summary>
        /// Adds middleware to handle incoming login requests using cookies to store auth token.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="cookieAuthOptions"></param>
        /// <param name="loginEndpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder RegisterCookieLoginMiddleware(this IApplicationBuilder applicationBuilder, ICookieAuthOptions cookieAuthOptions, ILoginEndpointOptions loginEndpointOptions)
        {
            return applicationBuilder
                .UseMiddleware<LoginWithCookieMiddleware>(cookieAuthOptions, loginEndpointOptions)
                .UseMiddleware<TokenExtractionFromCookieMiddleware>(cookieAuthOptions);
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the authentication starts.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IApplicationBuilder OnLoginExecuting(this IApplicationBuilder applicationBuilder, Func<BasicLoginModel, Task> callback)
        {
            return DefaultLoginMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnLoginExecuting(callback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when the authentication ends.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IApplicationBuilder OnLoginExecuted(this IApplicationBuilder applicationBuilder, Func<BasicLoginModel, Task> callback)
        {
            return DefaultLoginMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnLoginExecuted(callback);
        }
    }
}