using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login
{
    internal class LoginMiddlewareWithExecutedCallback : LoginMiddleware
    {
        public LoginMiddlewareWithExecutedCallback(RequestDelegate next, Func<BasicLoginModel, Task> onLoginExecuted)
            : base(next, null, null, onLoginExecuted) { }
    }
}