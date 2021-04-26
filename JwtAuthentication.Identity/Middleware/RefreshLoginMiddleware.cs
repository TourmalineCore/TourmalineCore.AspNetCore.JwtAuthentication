using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class RefreshLoginMiddleware<TUser> : AuthMiddlewareBase<IdentityRefreshLoginService<TUser>, RefreshLoginRequestModel, RefreshAuthResponseModel> where TUser : IdentityUser
    {
        public RefreshLoginMiddleware(RequestDelegate next)
            : base(next)
        {
        }

        public async Task InvokeAsync(HttpContext context, IdentityRefreshLoginService<TUser> loginService)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                var endpoint = context.Request.Path.Value;

                if (endpoint.EndsWith(loginService.GetRefreshTokenRoute()))
                {
                    try
                    {
                        var requestModel = await DeserializeModel<RefreshTokenRequestModel>(context.Request);

                        var result = new RefreshAuthResponseModel();

                        try
                        {
                            result = await loginService.RefreshTokenAsync(requestModel);
                        }
                        catch (AuthenticationException)
                        {
                            context.Response.StatusCode = 401;
                        }

                        if (result != null)
                        {
                            await Response(context, result);
                            return;
                        }
                    }
                    catch (AuthenticationException e)
                    {
                        throw new AuthenticationException(e.ExceptionInfo);
                    }
                }
            }

            await InvokeAsyncBase(context, loginService);
        }

        protected override async Task<RefreshAuthResponseModel> ExecuteServiceMethod(
            RefreshLoginRequestModel model,
            IdentityRefreshLoginService<TUser> service,
            HttpContext context)
        {
            var result = new RefreshAuthResponseModel();

            try
            {
                result = await service.LoginAsync(model);
            }
            catch (AuthenticationException)
            {
                context.Response.StatusCode = 401;
            }

            return result;
        }
    }
}