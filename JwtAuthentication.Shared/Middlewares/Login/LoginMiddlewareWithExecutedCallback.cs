using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login
{
    internal class LoginMiddlewareWithExecutedCallback : LoginMiddleware
    {
        public LoginMiddlewareWithExecutedCallback(RequestDelegate next, BaseLoginEndpointOptions loginEndpointOptions, Func<BasicLoginModel, Task> onLoginExecuted)
            : base(next, loginEndpointOptions, null, onLoginExecuted) { }
    }
}