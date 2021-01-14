using System;
using System.IdentityModel.Tokens.Jwt;
using JwtAuthentication.Core.InterfacesForUserImplementation;
using JwtAuthentication.Core.InterfacesForUserImplementation.DummyImplementations;
using JwtAuthentication.Core.Services;
using JwtAuthentication.Core.Services.Implementation;
using JwtAuthentication.Core.TokenHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Utils;
using AuthenticationOptions = JwtAuthentication.Core.Options.AuthenticationOptions;

namespace JwtAuthentication.Core
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the ability to use the basic functionality of JWT
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication<TKey>(
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions = null) where TKey : IEquatable<TKey>
        {
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserCredentialsValidator, DummyUserCredentialValidator>();

            services.AddJwtBearer(authenticationOptions);

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