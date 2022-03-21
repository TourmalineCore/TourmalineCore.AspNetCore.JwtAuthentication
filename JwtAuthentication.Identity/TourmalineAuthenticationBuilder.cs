using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public class TourmalineAuthenticationBuilder<TContext, TUser> : TourmalineAuthenticationBuilder<TContext, TUser, string>
        where TContext : TourmalineDbContext<TUser, string>
        where TUser : IdentityUser<string>
    {
        public TourmalineAuthenticationBuilder(IServiceCollection services, Action<IdentityOptions> setupAction = null)
            : base(services, setupAction)
        {
        }
    }

    public class TourmalineAuthenticationBuilder<TContext, TUser, TKey>
        where TContext : TourmalineDbContext<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private static IServiceCollection Services { get; set; }

        private static IdentityBuilder IdentityBuilder { get; set; }

        public TourmalineAuthenticationBuilder(IServiceCollection services, Action<IdentityOptions> setupAction = null)
        {
            Services = services;
            AddIdentity(setupAction);
        }

        /// <summary>
        /// Adds the user identity with db context
        /// </summary>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        private void AddIdentity(Action<IdentityOptions> setupAction = null)
        {
            Services.AddTransient<TourmalineDbContext<TUser, TKey>, TContext>();

            var setup = setupAction
                        ?? (options =>
                        {
                            options.Password.RequiredLength = 6;
                            options.Password.RequireLowercase = true;
                            options.Password.RequireUppercase = true;
                            options.Password.RequireDigit = true;
                            options.Password.RequireNonAlphanumeric = true;

                            options.Lockout.MaxFailedAccessAttempts = 10;
                            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(360);
                        });

            IdentityBuilder = Services
                .AddIdentityCore<TUser>(setup)
                .AddEntityFrameworkStores<TContext>();
        }

        /// <summary>
        /// Adds the ability to use the basic functionality of JWT authentication using Microsoft Identity to store and validate
        /// users data
        /// </summary>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddBaseLogin(AuthenticationOptions authenticationOptions = null)
        {
            AddJwt(Services, authenticationOptions);
            IdentityBuilder.AddSignInManager<SignInManager<TUser>>();

            Services.AddTransient<ITokenManager, TokenManager>();
            Services.AddTransient<ILoginService, IdentityLoginService<TUser, TKey>>();
            Services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            Services.AddTransient<IUserCredentialsValidator, IdentityUserCredentialsValidator<TUser, TKey>>();

            return this;
        }

        /// <summary>
        /// Adds the ability to handle incoming user registration requests with custom request model
        /// </summary>
        /// <typeparam name="TRegistrationRequestModel"></typeparam>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddRegistration<TRegistrationRequestModel>()
            where TRegistrationRequestModel : RegistrationRequestModel
        {
            Services.AddTransient<IRegistrationService<TUser, TRegistrationRequestModel>, IdentityRegistrationService<TUser, TKey, TRegistrationRequestModel>>();
            return this;
        }

        /// <summary>
        /// Adds the ability to handle incoming user registration requests
        /// </summary>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddRegistration()
        {
            Services.AddTransient<IRegistrationService<TUser, RegistrationRequestModel>, IdentityRegistrationService<TUser, TKey, RegistrationRequestModel>>();
            return this;
        }

        /// <summary>
        /// Adds the ability to use the functionality of JWT authentication using Microsoft Identity to store and validate users
        /// data and refresh tokens
        /// </summary>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddLoginWithRefresh(RefreshAuthenticationOptions authenticationOptions)
        {
            Services.AddSingleton(authenticationOptions);

            TourmalineContextConfiguration.UseRefresh = true;
            AddJwt(Services, authenticationOptions);
            IdentityBuilder.AddSignInManager<RefreshSignInManager<TUser, TKey>>();

            Services.AddTransient<ITokenManager, TokenManager>();
            Services.AddTransient<IRefreshTokenManager, RefreshTokenManager<TUser, TKey>>();
            Services.AddTransient<ILoginService, IdentityRefreshLoginService<TUser, TKey>>();
            Services.AddTransient<IRefreshService, IdentityRefreshLoginService<TUser, TKey>>();
            Services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            Services.AddTransient<IValidator<RefreshTokenRequestModel>, RefreshTokenValidator>();
            Services.AddTransient<IUserCredentialsValidator, IdentityUserCredentialsValidator<TUser, TKey>>();

            return this;
        }

        /// <summary>
        /// Allows to implement custom logic for checking the username and password
        /// </summary>
        /// <typeparam name="TUserCredentialsValidator"></typeparam>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddUserCredentialsValidator<TUserCredentialsValidator>()
            where TUserCredentialsValidator : IUserCredentialsValidator
        {
            Services.AddTransient(typeof(IUserCredentialsValidator), typeof(TUserCredentialsValidator));
            return this;
        }

        /// <summary>
        /// Adds the ability to implement functionality for retrieving user claims 
        /// </summary>
        /// <typeparam name="TUserClaimsProvider"></typeparam>
        /// <param name="permissionClaimTypeKey"></param>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> WithUserClaimsProvider<TUserClaimsProvider>(
            string permissionClaimTypeKey = "Permission")
            where TUserClaimsProvider : IUserClaimsProvider
        {
            RequiresPermission.ClaimType = permissionClaimTypeKey;

            Services.AddTransient(typeof(IUserClaimsProvider), typeof(TUserClaimsProvider));

            return this;
        }

        /// <summary>
        /// Adds the ability to handle incoming user logout requests
        /// </summary>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser, TKey> AddLogout()
        {
            Services.AddTransient<ILogoutService, IdentityLogoutService<TUser, TKey>>();
            return this;
        }

        private void AddJwt(
            IServiceCollection services,
            AuthenticationOptions authenticationOptions = null)
        {
            var options = authenticationOptions ?? new RefreshAuthenticationOptions();
            services.AddJwtBearer(options);
        }
    }
}