using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.InterfacesForUserImplementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.InterfacesForUserImplementation.DummyImplementations;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.TokenHandlers;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Utils;
using AuthenticationOptions = TourmalineCore.AspNetCore.JwtAuthentication.Core.Options.AuthenticationOptions;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the ability to use the basic functionality of JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions = null)
        {
            var options = authenticationOptions ?? new AuthenticationOptions();

            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserCredentialsValidator, DummyUserCredentialValidator>();

            services.AddJwtBearer(options);

            return services;
        }

        /// <summary>
        /// Allows to implement custom logic for checking the username and password
        /// </summary>
        /// <typeparam name="TUserCredentialsValidator"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserCredentialValidator<TUserCredentialsValidator>(this IServiceCollection services)
            where TUserCredentialsValidator : IUserCredentialsValidator
        {
            return services.AddTransient(typeof(IUserCredentialsValidator), typeof(TUserCredentialsValidator));
        }

        /// <summary>
        /// Allows to change the default route for calling the login
        /// </summary>
        /// <param name="services"></param>
        /// <param name="newRoute"></param>
        /// <returns></returns>
        public static IServiceCollection OverrideLoginRoute(this IServiceCollection services, string newRoute)
        {
            LoginService.OverrideRoute(newRoute);

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {

            //services.AddScoped<IUserStore<User>, CustomUserStore<User>>();
            //services.AddScoped<IRoleStore<Role>, CustomRoleStore>();

            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddRoleManager<CustomRoleManager>()
                .AddUserManager<CustomUserManager>()
                .AddRoleStore<CustomRoleStore<Role>>()
                .AddUserStore<CustomUserStore<User>>()
                .AddDefaultTokenProviders();

            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IRoleCreationService, RoleCreationService>();

            return services;
        }

        private static void AddJwtBearer(this IServiceCollection services, AuthenticationOptions authenticationOptions)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            Func<HttpContext, string> schemeSelector = context => null;

            var authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            if (authenticationOptions.IsDebugTokenEnabled)
            {
                authBuilder.AddScheme<AuthenticationSchemeOptions, DebugTokenHandler>(DebugTokenHandler.Schema,
                        options => { }
                    );

                schemeSelector = context =>
                {
                    string debugHeader = context.Request.Headers[DebugTokenHandler.HeaderName];

                    return string.IsNullOrWhiteSpace(debugHeader) == false
                        ? DebugTokenHandler.Schema
                        : null;
                };
            }

            authBuilder
                .AddJwtBearer(
                        options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateLifetime = true,
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(authenticationOptions.SigningKey.ToEncodedByteArray()),
                                ClockSkew = TimeSpan.Zero,
                            };

                            options.ForwardDefaultSelector = schemeSelector;
                        }
                    );
        }
    }
}