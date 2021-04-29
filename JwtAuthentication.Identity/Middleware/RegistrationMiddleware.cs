using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class RegistrationMiddleware<TUser, TRegistrationRequestModel> : RequestMiddlewareBase<IRegistrationService<TUser, TRegistrationRequestModel>, TRegistrationRequestModel, AuthResponseModel
    >
        where TUser : IdentityUser
        where TRegistrationRequestModel : RegistrationRequestModel
    {
        private readonly RegistrationEndpointOptions _endpointOptions;
        private readonly Func<TRegistrationRequestModel, TUser> _mapping;

        public RegistrationMiddleware(
            RequestDelegate next,
            Func<TRegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions endpointOptions = null)
            : base(next)
        {
            _mapping = mapping;
            _endpointOptions = endpointOptions;
        }

        public async Task InvokeAsync(HttpContext context, IRegistrationService<TUser, TRegistrationRequestModel> registrationService)
        {
            await InvokeAsyncBase(context, registrationService, _endpointOptions.RegistrationEndpointRoute);
        }

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(TRegistrationRequestModel model, IRegistrationService<TUser, TRegistrationRequestModel> service, HttpContext context)
        {
            var result = new AuthResponseModel();

            try
            {
                result = await service.RegisterAsync(model, _mapping);
            }
            catch (RegistrationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            return result;
        }
    }
}