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
        public static TourmalineAuthenticationBuilder<TContext, TUser, string> AddJwtAuthenticationWithIdentity<TContext, TUser>(
            this IServiceCollection services,
            Action<IdentityOptions> setupAction = null)
            where TContext : TourmalineDbContext<TUser, string>
            where TUser : IdentityUser
        {
            return AddJwtAuthenticationWithIdentity<TContext, TUser, string>(services, setupAction);
        }

        /// <summary>
        /// Adds the ability to use the basic functionality of JWT authentication using Microsoft Identity to store and validate
        /// users data with generic primary key
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddJwtAuthenticationWithIdentity<TContext, TUser, TKey>(
            this IServiceCollection services,
            Action<IdentityOptions> setupAction = null)
            where TContext : TourmalineDbContext<TUser, TKey>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            });

            return new TourmalineAuthenticationBuilder<TContext, TUser, TKey>(services, setupAction);
        }


    }
}