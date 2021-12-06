using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Registration;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Registration.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using IRefreshMiddlewareBuilder = TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.IRefreshMiddlewareBuilder;
using RefreshMiddlewareBuilder = TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.RefreshMiddlewareBuilder;

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
            where TContext : TourmalineDbContext<TUser> 
            where TUser : IdentityUser
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
        /// Adds a user with generic id to the database with specified credentials
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="applicationBuilder"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultDbUser<TContext, TUser, TKey>(
            this IApplicationBuilder applicationBuilder,
            string username,
            string password)
            where TContext : TourmalineDbContext<TUser, TKey>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

            var user = Activator.CreateInstance<TUser>();

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
            Func<RefreshModel, Task> defaultOnRefreshCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<RefreshMiddleware>(endpointOptions ?? new RefreshEndpointOptions(), defaultOnRefreshCallback, defaultOnRefreshCallback);
        }

        /// <summary>
        /// Adds middleware to handle incoming user registration requests. It requires a function to map model received from client
        /// to user entity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
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
            Func<RegistrationModel, Task> defaultOnRegistrationCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, RegistrationRequestModel>>(
                        mapping,
                        defaultOnRegistrationCallback,
                        defaultOnRegistrationCallback,
                        options
                    );
        }      
        
        public static IApplicationBuilder UseRegistration<TUser, TKey>(
            this IApplicationBuilder applicationBuilder,
            Func<RegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            var options = registrationEndpointOptions ?? new RegistrationEndpointOptions();
            Func<RegistrationModel, Task> defaultOnRegistrationCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, TKey, RegistrationRequestModel>>(
                        mapping,
                        defaultOnRegistrationCallback,
                        defaultOnRegistrationCallback,
                        options
                    );
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
            Func<RegistrationModel, Task> defaultOnRegistrationCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, TRegistrationRequestModel>>(
                        mapping,
                        defaultOnRegistrationCallback,
                        defaultOnRegistrationCallback,
                        options
                    );
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
        public static IApplicationBuilder UseRegistration<TUser, TKey, TRegistrationRequestModel>(
            this IApplicationBuilder applicationBuilder,
            Func<TRegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TRegistrationRequestModel : RegistrationRequestModel
        {
            var options = registrationEndpointOptions ?? new RegistrationEndpointOptions();
            Func<RegistrationModel, Task> defaultOnRegistrationCallback = s => Task.CompletedTask;

            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware<TUser, TKey, TRegistrationRequestModel>>(
                        mapping,
                        defaultOnRegistrationCallback,
                        defaultOnRegistrationCallback,
                        options
                    );
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

        /// <summary>
        /// Registering a callback function to perform actions when  when the refresh starts.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IRegistrationMiddlewareBuilder OnRegistrationExecuting(this IApplicationBuilder applicationBuilder, Func<RegistrationModel, Task> callback)
        {
            return RegistrationMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnRegistrationExecuting(callback);
        }

        /// <summary>
        /// Registering a callback function to perform actions when the refresh ends.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IRegistrationMiddlewareBuilder OnRegistrationExecuted(this IApplicationBuilder applicationBuilder, Func<RegistrationModel, Task> callback)
        {
            return RegistrationMiddlewareBuilder
                .GetInstance()
                .SetAppBuilder(applicationBuilder)
                .OnRegistrationExecuted(callback);
        }
    }
}