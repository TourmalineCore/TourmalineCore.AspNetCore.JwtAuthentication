using System;
using JwtAuthentication.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation;

namespace JwtAuthentication.Identity
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
            where TContext : JwtAuthIdentityDbContext<TUser> where TUser : IdentityUser
        {
            var options = authenticationOptions ?? new AuthenticationOptions();

            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ILoginService, IdentityLoginService<TUser>>();
            services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            services.AddJwtBearer(options);

            services
                //ToDo: add possibility to provide custom options
                .AddIdentityCore<TUser>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;

                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(360);

                })
                .AddEntityFrameworkStores<TContext>()
                .AddSignInManager<SignInManager<TUser>>();


            return services;
        }
    }
}