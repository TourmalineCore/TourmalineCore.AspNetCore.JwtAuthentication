using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared
{
    public static class ApplicationBuilderExtension
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
        public static IApplicationBuilder UseDefaultLoginMiddleware(this IApplicationBuilder applicationBuilder, BaseLoginEndpointOptions loginEndpointOptions = null)
        {
            return DefaultLoginMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .SetLoginEndpointOptions(loginEndpointOptions)
                .UseDefaultLoginMiddleware();

            //Func<BasicLoginModel, Task> defaultOnLoginCallback = s => Task.CompletedTask;

            //return applicationBuilder
            //    .UseMiddleware<LoginMiddleware>(loginEndpointOptions ?? new BaseLoginEndpointOptions(), defaultOnLoginCallback, defaultOnLoginCallback);
        }

        ///// <summary>
        ///// Adds middleware to handle incoming login requests using cookies to store auth token.
        ///// </summary>
        ///// <param name="applicationBuilder"></param>
        ///// <param name="options"></param>
        ///// <returns></returns>
        //public static IApplicationBuilder UseCookieLoginMiddleware(this IApplicationBuilder applicationBuilder, CookieAuthOptions options, LoginEndpointOptions loginEndpointOptions = null)
        //{
        //    return applicationBuilder
        //        .UseMiddleware<LoginWithCookieMiddleware>(options, loginEndpointOptions ?? new LoginEndpointOptions())
        //        .UseMiddleware<TokenExtractionFromCookieMiddleware>(options);
        //}

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

        ///// <summary>
        ///// Adds middleware to handle incoming login and token refresh requests.
        ///// </summary>
        ///// <param name="applicationBuilder"></param>
        ///// <param name="endpointOptions"></param>
        ///// <returns></returns>
        //public static IApplicationBuilder UseRefreshTokenMiddleware(this IApplicationBuilder applicationBuilder, RefreshEndpointOptions endpointOptions = null)
        //{
        //    Func<RefreshModel, Task> defaultOnRefreshCallback = s => Task.CompletedTask;

        //    return applicationBuilder
        //        .UseMiddleware<RefreshMiddleware>(endpointOptions ?? new RefreshEndpointOptions(), defaultOnRefreshCallback, defaultOnRefreshCallback);
        //}

        ///// <summary>
        ///// Registering a callback function to perform actions when  when the refresh starts.
        ///// </summary>
        ///// <param name="applicationBuilder"></param>
        ///// <param name="callback"></param>
        ///// <returns></returns>
        //public static IRefreshMiddlewareBuilder OnRefreshExecuting(this IApplicationBuilder applicationBuilder, Func<RefreshModel, Task> callback)
        //{
        //    return RefreshMiddlewareBuilder
        //        .GetInstance()
        //        .SetAppBuilder(applicationBuilder)
        //        .OnRefreshExecuting(callback);
        //}

        ///// <summary>
        ///// Registering a callback function to perform actions when the refresh ends.
        ///// </summary>
        ///// <param name="applicationBuilder"></param>
        ///// <param name="callback"></param>
        ///// <returns></returns>
        //public static IRefreshMiddlewareBuilder OnRefreshExecuted(this IApplicationBuilder applicationBuilder, Func<RefreshModel, Task> callback)
        //{
        //    return RefreshMiddlewareBuilder
        //        .GetInstance()
        //        .SetAppBuilder(applicationBuilder)
        //        .OnRefreshExecuted(callback);
        //}
    }
}