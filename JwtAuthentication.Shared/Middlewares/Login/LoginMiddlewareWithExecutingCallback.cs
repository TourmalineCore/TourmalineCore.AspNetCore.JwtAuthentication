using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login
{
    internal class LoginMiddlewareWithExecutingCallback : LoginMiddleware
    {
        public LoginMiddlewareWithExecutingCallback(RequestDelegate next, Func<BasicLoginModel, Task> onLoginExecuting)
            : base(next, null, onLoginExecuting, null) { }      
    }
}