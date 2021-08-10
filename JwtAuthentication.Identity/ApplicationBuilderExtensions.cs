using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a user to the database with specified credentials
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="applicationBuilder"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultDbUser<TContext, TUser>(
            this IApplicationBuilder applicationBuilder,
            string username,
            string password)
            where TContext : TourmalineDbContext<TUser> where TUser : IdentityUser
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

            var user = Activator.CreateInstance<TUser>();

            user.Id = Guid.NewGuid().ToString();
            user.UserName = username;
            user.NormalizedUserName = username.ToUpper();
            user.EmailConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString();

            user.PasswordHash = new PasswordHasher<TUser>().HashPassword(user, password);

            context.Users.Add(user);
            context.SaveChanges();

            return applicationBuilder
                .UseAuthentication()
                .UseAuthorization();
        }

        /// <summary>
        /// Adds middleware to handle incoming login and token refresh requests.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="endpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRefreshTokenMiddleware(this IApplicationBuilder applicationBuilder, RefreshEndpointOptions endpointOptions = null)
        {
            return applicationBuilder
                .UseMiddleware<RefreshMiddleware>(endpointOptions ?? new RefreshEndpointOptions());
        }

        /// <summary>
        /// Adds middleware to handle incoming user registration requests with custom registration request model. It requires a
        /// function to map model received from client.
        /// to user entity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRegistrationRequestModel"></typeparam>
        /// <param name="applicationBuilder"></param>
        /// <param name="mapping"></param>
        /// <param name="registrationEndpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRegistration<TUser, TRegistrationRequestModel>(
            this IApplicationBuilder applicationBuilder,
            Func<TRegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser
            where TRegistrationRequestModel : RegistrationRequestModel
        {
            var options = registrationEndpointOptions ?? new RegistrationEndpointOptions();

            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, TRegistrationRequestModel>>(mapping, options);
        }

        /// <summary>
        /// Adds middleware to handle incoming user registration requests. It requires a function to map model received from client
        /// to user entity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRegistrationRequestModel"></typeparam>
        /// <param name="applicationBuilder"></param>
        /// <param name="mapping"></param>
        /// <param name="registrationEndpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRegistration<TUser>(
            this IApplicationBuilder applicationBuilder,
            Func<RegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser
        {
            var options = registrationEndpointOptions ?? new RegistrationEndpointOptions();

            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, RegistrationRequestModel>>(mapping, options);
        }

        /// <summary>
        /// Adds middleware to handle incoming logout requests.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="endpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRefreshTokenLogoutMiddleware(this IApplicationBuilder applicationBuilder, LogoutEndpointOptions endpointOptions = null)
        {
            Func<LogoutModel, Task> defaultOnLogoutCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<LogoutMiddleware>(endpointOptions ?? new LogoutEndpointOptions(), defaultOnLogoutCallback, defaultOnLogoutCallback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when  when the logout starts.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ILogoutMiddlewareBuilder OnLogoutExecuting(this IApplicationBuilder applicationBuilder, Func<LogoutModel, Task> callback)
        {
            return LogoutMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnLogoutExecuting(callback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when the logout ends.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ILogoutMiddlewareBuilder OnLogoutExecuted(this IApplicationBuilder applicationBuilder, Func<LogoutModel, Task> callback)
        {
            return LogoutMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnLogoutExecuted(callback);
        }
    }
}