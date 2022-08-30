using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.TokenHandlers;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Signing;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;
using AuthenticationOptions = TourmalineCore.AspNetCore.JwtAuthentication.Core.Options.AuthenticationOptions;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Contracts;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core
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
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions)
        {
            return Shared.AuthenticationExtensions.AddJwtValidation(services, authenticationOptions);
        }

        /// <summary>
        /// Adds the ability to use the basic functionality of JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions)
        {
            return Shared.AuthenticationExtensions.AddJwtAuthentication(services, authenticationOptions);
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
            return Shared.AuthenticationExtensions.AddUserCredentialValidator<TUserCredentialsValidator>(services);
        }

        /// <summary>
        /// Adds the ability to implement functionality for retrieving user claims
        /// </summary>
        /// <typeparam name="TUserClaimsProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="permissionClaimTypeKey"></param>
        /// <returns></returns>
        public static IServiceCollection WithUserClaimsProvider<TUserClaimsProvider>(
            this IServiceCollection services,
            string permissionClaimTypeKey = "Permission")
            where TUserClaimsProvider : UserClaimsProvider
        {
            RequiresPermission.ClaimType = permissionClaimTypeKey;
            return Shared.AuthenticationExtensions.WithUserClaimsProvider<TUserClaimsProvider>(services);
        }

        public static IServiceCollection AddLoginWithRefresh(
            this IServiceCollection services,
            RefreshTokenOptions refreshTokenOptions = null)
        {
            services.AddSingleton(refreshTokenOptions ?? new RefreshTokenOptions());

            services.AddTransient<ILoginService, LoginWithRefreshService>();
            services.AddTransient<IJwtTokenValidator, JwtTokenValidator>();
            services.AddTransient<ICoreRefreshTokenManager, CoreRefreshTokenManager>();
            services.AddTransient<ICoreRefreshService, CoreRefreshService>();

            return services;
        }

        internal static void AddJwtBearer(
            this IServiceCollection services,
            AuthenticationOptions authenticationOptions)
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