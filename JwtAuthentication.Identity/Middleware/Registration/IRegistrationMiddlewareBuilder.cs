using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Registration.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Registration
{
    public interface IRegistrationMiddlewareBuilder
    {
        public IRegistrationMiddlewareBuilder OnRegistrationExecuting(Func<RegistrationModel, Task> callback);

        public IRegistrationMiddlewareBuilder OnRegistrationExecuted(Func<RegistrationModel, Task> callback);

        public IApplicationBuilder UseRegistration<TUser, TRegistrationRequestModel>(
            Func<TRegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser
            where TRegistrationRequestModel : RegistrationRequestModel;

        public IApplicationBuilder UseRegistration<TUser>(
            Func<RegistrationRequestModel, TUser> mapping,
            RegistrationEndpointOptions registrationEndpointOptions = null)
            where TUser : IdentityUser;
    }
}