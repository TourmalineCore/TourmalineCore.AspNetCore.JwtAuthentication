using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class RegistrationMiddleware : AuthMiddlewareBase<IRegistrationService, RegistrationRequestModel, string>
    {
        public RegistrationMiddleware(RequestDelegate next)
            : base(next)
        {
        }

        public async Task InvokeAsync(HttpContext context, IRegistrationService registrationService)
        {
            await InvokeAsyncBase(context, registrationService);
        }

        protected override Task<string> ExecuteServiceMethod(RegistrationRequestModel model, IRegistrationService service)
        {
            return service.RegistrationAsync(model);
        }
    }
}