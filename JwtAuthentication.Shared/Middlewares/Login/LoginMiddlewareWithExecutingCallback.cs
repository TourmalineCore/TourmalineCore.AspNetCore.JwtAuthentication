using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login
{
    internal class LoginMiddlewareWithExecutingCallback : LoginMiddleware
    {
        public LoginMiddlewareWithExecutingCallback(RequestDelegate next, BaseLoginEndpointOptions loginEndpointOptions, Func<BasicLoginModel, Task> onLoginExecuting)
            : base(next, loginEndpointOptions, onLoginExecuting, null) { }      
    }
}