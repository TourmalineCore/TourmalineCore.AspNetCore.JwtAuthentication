using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RegistrationMiddleware<TUser, TRegistrationRequestModel>> _logger;

        public RegistrationMiddleware(
            RequestDelegate next,
            Func<TRegistrationRequestModel, TUser> mapping, ILogger<RegistrationMiddleware<TUser, TRegistrationRequestModel>> logger, RegistrationEndpointOptions endpointOptions = null)
            : base(next)
        {
            _mapping = mapping;
            _logger = logger;
            _endpointOptions = endpointOptions;
        }

        public async Task InvokeAsync(HttpContext context, IRegistrationService<TUser, TRegistrationRequestModel> registrationService)
        {
            await InvokeAsyncBase(context, registrationService, _endpointOptions.RegistrationEndpointRoute);
        }

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(TRegistrationRequestModel requestModel, IRegistrationService<TUser, TRegistrationRequestModel> service, HttpContext context)
        {
            var result = new AuthResponseModel();

            try
            {
                result = await service.RegisterAsync(requestModel, _mapping);
            }
            catch (RegistrationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                _logger.LogError(ex.ToString());
            }

            return result;
        }
    }
}