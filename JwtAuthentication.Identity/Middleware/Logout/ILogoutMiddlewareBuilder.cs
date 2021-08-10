using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout
{
    public interface ILogoutMiddlewareBuilder
    {
        public ILogoutMiddlewareBuilder OnLogoutExecuting(Func<LogoutModel, Task> callback);

        public ILogoutMiddlewareBuilder OnLogoutExecuted(Func<LogoutModel, Task> callback);

        public IApplicationBuilder UseLogoutMiddleware(LogoutEndpointOptions endpointOptions = null);
    }
}