using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the ability to use the basic functionality of JWT authentication using Microsoft Identity to store and validate users data
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationWithIdentity<TContext, TUser>(
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions = null
            ) 
            where TContext : JwtAuthIdentityDbContext<TUser> 
            where TUser : IdentityUser
        {
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ILoginService, IdentityLoginService<TUser>>();
            services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            services.AddTransient<JwtAuthIdentityDbContext<TUser>, TContext>();

            services.AddJwt(authenticationOptions);
            services.AddIdentity<TContext, TUser, SignInManager<TUser>>();

            return services;
        }

        /// <summary>
        /// Adds the ability to handle incoming user registration requests
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRegistrationRequestModel"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRegistration<TUser, TRegistrationRequestModel>(
                this IServiceCollection services
            )
            where TUser : IdentityUser
            where TRegistrationRequestModel : RegistrationRequestModel
        {
            services.AddTransient<IRegistrationService<TUser, TRegistrationRequestModel>, IdentityRegistrationService<TUser, TRegistrationRequestModel>>();

            return services;
        }

        /// <summary>
        /// Adds the ability to use the functionality of JWT authentication using Microsoft Identity to store and validate users data and refresh tokens
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthenticationWithRefreshToken<TContext, TUser>(
                this IServiceCollection services,
                RefreshAuthenticationOptions authenticationOptions = null
            )
            where TContext : JwtAuthIdentityRefreshTokenDbContext<TUser> where TUser : IdentityUser
        {
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<IRefreshTokenManager, RefreshTokenManager<TUser>>();
            services.AddTransient<ILoginService, IdentityRefreshLoginService<TUser>>();
            services.AddTransient<IRefreshService, IdentityRefreshLoginService<TUser>>();
            services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            services.AddTransient<JwtAuthIdentityRefreshTokenDbContext<TUser>, TContext>();

            services.AddTransient<IValidator<RefreshTokenRequestModel>, RefreshTokenValidator>();

            services.AddJwt(authenticationOptions);
            services.AddIdentity<TContext, TUser, RefreshSignInManager<TUser>>();

            return services;
        }

        private static IServiceCollection AddJwt(
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions = null)
        {
            var options = authenticationOptions ?? new RefreshAuthenticationOptions();
            services.AddJwtBearer(options);

            return services;
        }

        private static IServiceCollection AddIdentity<TContext, TUser, TSignInManager>(this IServiceCollection services)
            where TContext : JwtAuthIdentityDbContext<TUser> 
            where TUser : IdentityUser
            where TSignInManager : SignInManager<TUser>
        {
            services
                //ToDo: #13: add possibility to provide custom options
                .AddIdentityCore<TUser>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(360);

                })
                .AddEntityFrameworkStores<TContext>()
                .AddSignInManager<TSignInManager>();

            return services;
        }
    }
}