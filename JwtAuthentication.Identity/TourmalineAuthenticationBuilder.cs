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
    public class TourmalineAuthenticationBuilder<TContext, TUser>
        where TContext : TourmalineDbContext<TUser>
        where TUser : IdentityUser
    {
        private static IServiceCollection Services { get; set; }

        private static IdentityBuilder IdentityBuilder { get; set; }

        public TourmalineAuthenticationBuilder(IServiceCollection services, Action<IdentityOptions> setupAction = null)
        {
            Services = services;
            AddIdentity(setupAction);
        }

        private void AddIdentity(Action<IdentityOptions> setupAction = null)
        {
            Services.AddTransient<TourmalineDbContext<TUser>, TContext>();

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
        public TourmalineAuthenticationBuilder<TContext, TUser> AddBaseLogin(AuthenticationOptions authenticationOptions = null)
        {
            AddJwt(Services, authenticationOptions);
            IdentityBuilder.AddSignInManager<SignInManager<TUser>>();

            Services.AddTransient<ITokenManager, TokenManager>();
            Services.AddTransient<ILoginService, IdentityLoginService<TUser>>();
            Services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            Services.AddTransient<IUserCredentialsValidator, IdentityUserCredentialsValidator<TUser>>();

            return this;
        }

        /// <summary>
        /// Adds the ability to handle incoming user registration requests with custom request model
        /// </summary>
        /// <typeparam name="TRegistrationRequestModel"></typeparam>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser> AddRegistration<TRegistrationRequestModel>()
            where TRegistrationRequestModel : RegistrationRequestModel
        {
            Services.AddTransient<IRegistrationService<TUser, TRegistrationRequestModel>, IdentityRegistrationService<TUser, TRegistrationRequestModel>>();
            return this;
        }

        /// <summary>
        /// Adds the ability to handle incoming user registration requests
        /// </summary>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser> AddRegistration()
        {
            Services.AddTransient<IRegistrationService<TUser, RegistrationRequestModel>, IdentityRegistrationService<TUser, RegistrationRequestModel>>();
            return this;
        }

        /// <summary>
        /// Adds the ability to use the functionality of JWT authentication using Microsoft Identity to store and validate users
        /// data and refresh tokens
        /// </summary>
        /// <param name="authenticationOptions"></param>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser> AddLoginWithRefresh(RefreshAuthenticationOptions authenticationOptions)
        {
            Services.AddSingleton(authenticationOptions);

            TourmalineContextConfiguration.UseRefresh = true;
            AddJwt(Services, authenticationOptions);
            IdentityBuilder.AddSignInManager<RefreshSignInManager<TUser>>();

            Services.AddTransient<ITokenManager, TokenManager>();
            Services.AddTransient<IRefreshTokenManager<TUser>, RefreshTokenManager<TUser>>();
            Services.AddTransient<ILoginService, IdentityRefreshLoginService<TUser>>();
            Services.AddTransient<IRefreshService, IdentityRefreshLoginService<TUser>>();
            Services.AddTransient<IUserClaimsProvider, DefaultUserClaimsProvider>();
            Services.AddTransient<IValidator<RefreshTokenRequestModel>, RefreshTokenValidator>();
            Services.AddTransient<IUserCredentialsValidator, IdentityUserCredentialsValidator<TUser>>();

            return this;
        }

        /// <summary>
        /// Allows to implement custom logic for checking the username and password
        /// </summary>
        /// <typeparam name="TUserCredentialsValidator"></typeparam>
        /// <returns></returns>
        public TourmalineAuthenticationBuilder<TContext, TUser> AddUserCredentialsValidator<TUserCredentialsValidator>()
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
        public TourmalineAuthenticationBuilder<TContext, TUser> WithUserClaimsProvider<TUserClaimsProvider>(
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
        public TourmalineAuthenticationBuilder<TContext, TUser> AddLogout()
        {
            Services.AddTransient<ILogoutService, IdentityLogoutService<TUser>>();
            Services.AddTransient<IRefreshService, IdentityRefreshLoginService<TUser>>();
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