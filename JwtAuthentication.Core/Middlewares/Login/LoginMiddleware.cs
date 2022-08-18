using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login
{
    internal class LoginMiddleware : RequestMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        private readonly LoginEndpointOptions _loginEndpointOptions;

        private readonly Func<LoginModel, Task> _onLoginExecuting;
        private readonly Func<LoginModel, Task> _onLoginExecuted;

        public LoginMiddleware(RequestDelegate next, LoginEndpointOptions loginEndpointOptions, Func<LoginModel, Task> onLoginExecuting, Func<LoginModel, Task> onLoginExecuted)
            : base(next)
        {
            _loginEndpointOptions = loginEndpointOptions;
            _onLoginExecuting = onLoginExecuting;
            _onLoginExecuted = onLoginExecuted;
        }

        public async Task InvokeAsync(HttpContext context, ILoginService loginService)
        {
            await InvokeAsyncBase(context, loginService, _loginEndpointOptions.LoginEndpointRoute);
        }

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(
            LoginRequestModel requestModel,
            ILoginService service,
            HttpContext context)
        {
            var result = new AuthResponseModel();

            try
            {
                var contractLoginModel = new LoginModel
                {
                    Login = requestModel.Login,
                    Password = requestModel.Password,
                    ClientFingerPrint = requestModel.ClientFingerPrint,
                };

                await _onLoginExecuting(contractLoginModel);
                result = await service.LoginAsync(requestModel);
                await _onLoginExecuted(contractLoginModel);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                throw new Exception(ex.Message);
            }

            return result;
        }
    }
}