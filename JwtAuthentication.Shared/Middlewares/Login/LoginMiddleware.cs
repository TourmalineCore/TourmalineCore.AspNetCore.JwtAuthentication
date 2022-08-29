using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login
{
    internal class LoginMiddleware : RequestMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        protected static BaseLoginEndpointOptions _loginEndpointOptions;
        protected static Func<BasicLoginModel, Task> _onLoginExecuting = null;
        protected static Func<BasicLoginModel, Task> _onLoginExecuted = null;

        public LoginMiddleware(RequestDelegate next, BaseLoginEndpointOptions loginEndpointOptions, Func<BasicLoginModel, Task> onLoginExecuting = null, Func<BasicLoginModel, Task> onLoginExecuted = null)
            : base(next)
        {
            _loginEndpointOptions = loginEndpointOptions;
            
            if (onLoginExecuting != null)
            {
                _onLoginExecuting = onLoginExecuting;
            }

            if (onLoginExecuted != null)
            {
                _onLoginExecuted = onLoginExecuted;
            }
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
                var contractLoginModel = new BasicLoginModel
                {
                    Login = requestModel.Login,
                    Password = requestModel.Password,
                    ClientFingerPrint = requestModel.ClientFingerPrint,
                };

                if (_onLoginExecuting != null)
                {
                    await _onLoginExecuting(contractLoginModel);
                }

                result = await service.LoginAsync(requestModel);

                if (_onLoginExecuted != null)
                {
                    await _onLoginExecuted(contractLoginModel);
                }                
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