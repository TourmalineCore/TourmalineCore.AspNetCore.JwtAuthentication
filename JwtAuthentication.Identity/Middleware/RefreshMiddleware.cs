using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class RefreshMiddleware: AuthMiddlewareBase<ILoginService, LoginRequestModel, AuthResponseModel>
    {
        public RefreshMiddleware(RequestDelegate next)
            : base(next)
        {
        }

        public async Task InvokeAsync(HttpContext context, ILoginService loginService)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                var endpoint = context.Request.Path.Value;

                if (endpoint.EndsWith(loginService.GetRefreshTokenRoute()))
                {
                    try
                    {
                        var requestModel = await DeserializeModel<RefreshTokenRequestModel>(context.Request);

                        var result = new AuthResponseModel();

                        try
                        {
                            result = await loginService.RefreshAsync(requestModel);
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

        protected override async Task<AuthResponseModel> ExecuteServiceMethod(
            LoginRequestModel model,
            ILoginService service,
            HttpContext context)
        {
            var result = new AuthResponseModel();

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