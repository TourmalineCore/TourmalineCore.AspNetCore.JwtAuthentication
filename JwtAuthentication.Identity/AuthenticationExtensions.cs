using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the ability to use the basic functionality of JWT authentication using Microsoft Identity to store and validate
        /// users data
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static TourmalineAuthenticationBuilder<TContext, TUser> AddJwtAuthenticationWithIdentity<TContext, TUser>(
            this IServiceCollection services,
            Action<IdentityOptions> setupAction = null)
            where TContext : TourmalineDbContext<TUser>
            where TUser : IdentityUser
        {
            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });

            return new TourmalineAuthenticationBuilder<TContext, TUser>(services, setupAction);
        }
    }
}