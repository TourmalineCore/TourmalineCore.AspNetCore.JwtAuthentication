using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Signing;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenHandlers;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the ability to validate JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtValidation(
            IServiceCollection services,
            IAuthenticationOptions authenticationOptions)
        {
            services.AddJwtBearer(authenticationOptions);

            return services;
        }

        /// <summary>
        /// Adds the ability to use the basic functionality of JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IAuthenticationOptions authenticationOptions)
        {
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserCredentialsValidator, FakeUserCredentialValidator>();
            services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            services.AddTransient<IJwtTokenCreator, JwtTokenCreator>();

            services.AddJwtBearer(authenticationOptions);

            return services;
        }

        /// <summary>
        /// Allows to implement custom logic for checking the username and password
        /// </summary>
        /// <typeparam name="TUserCredentialsValidator"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserCredentialValidator<TUserCredentialsValidator>(IServiceCollection services)
            where TUserCredentialsValidator : IUserCredentialsValidator
        {
            return services.AddTransient(typeof(IUserCredentialsValidator), typeof(TUserCredentialsValidator));
        }

        /// <summary>
        /// Adds the ability to implement functionality for retrieving user claims
        /// </summary>
        /// <typeparam name="TUserClaimsProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="permissionClaimTypeKey"></param>
        /// <returns></returns>
        public static IServiceCollection WithUserClaimsProvider<TUserClaimsProvider>(IServiceCollection services)
            where TUserClaimsProvider : IUserClaimsProvider
        {
            return services.AddTransient(typeof(IUserClaimsProvider), typeof(TUserClaimsProvider));
        }

        public static void AddJwtBearer(
            this IServiceCollection services,
            IAuthenticationOptions authenticationOptions)
        {
            services.AddSingleton(authenticationOptions);

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
                                IssuerSigningKey = SigningHelper.GetPublicKey(authenticationOptions.PublicSigningKey),
                                ClockSkew = TimeSpan.Zero,
                            };

                            options.ForwardDefaultSelector = schemeSelector;
                        }
                    );
        }
    }
}