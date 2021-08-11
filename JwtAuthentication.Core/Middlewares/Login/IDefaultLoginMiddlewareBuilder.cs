using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login
{
    public interface IDefaultLoginMiddlewareBuilder
    {
        public IDefaultLoginMiddlewareBuilder OnLoginExecuting(Func<LoginModel, Task> callback);

        public IDefaultLoginMiddlewareBuilder OnLoginExecuted(Func<LoginModel, Task> callback);

        public IApplicationBuilder UseDefaultLoginMiddleware(LoginEndpointOptions loginEndpointOptions = null);
    }
}