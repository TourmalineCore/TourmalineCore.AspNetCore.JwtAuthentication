using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal abstract class AuthMiddlewareBase<TService, TRequestModel, TResponseModel> where TService : IAuthService
    {
        private readonly RequestDelegate _next;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public AuthMiddlewareBase(RequestDelegate next)
        {
            _next = next;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                Converters =
                {
                    new IsoDateTimeConverter(),
                    new StringEnumConverter(),
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public async Task InvokeAsyncBase(HttpContext context, TService service)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                var endpoint = context.Request.Path.Value;

                if (endpoint.EndsWith(service.GetRoute()))
                {
                    try
                    {
                        var requestModel = await DeserializeModel<TRequestModel>(context.Request);

                        var result = await ExecuteServiceMethod(requestModel, service);

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

            await _next(context);
        }

        private async Task Response(HttpContext context, TResponseModel result)
        {
            context.Response.ContentType = "application/json; charset=UTF-8";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result, _jsonSerializerSettings));
            await context.Response.Body.FlushAsync();
        }

        private async Task<T> DeserializeModel<T>(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            {
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync(), _jsonSerializerSettings);
            }
        }

        protected abstract Task<TResponseModel> ExecuteServiceMethod(TRequestModel model, TService service);
    }
}