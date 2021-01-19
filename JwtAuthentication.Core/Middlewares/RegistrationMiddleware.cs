using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class RegistrationMiddleware : AuthMiddlewareBase<IRegistrationService, RegistrationRequestModel, long>
    {
        public RegistrationMiddleware(RequestDelegate next)
            : base(next)
        {
        }

        public async Task InvokeAsync(HttpContext context, IRegistrationService registrationService)
        {
            await InvokeAsyncBase(context, registrationService);
        }

        protected override Task<long> ExecuteServiceMethod(RegistrationRequestModel model, IRegistrationService service)
        {
            return service.RegistrationAsync(model);
        }
    }
}