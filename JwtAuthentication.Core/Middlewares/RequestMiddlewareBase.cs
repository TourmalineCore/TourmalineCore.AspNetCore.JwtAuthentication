using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal abstract class RequestMiddlewareBase<TService, TRequestModel, TResponseModel>
    {
        private readonly RequestDelegate _next;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public RequestMiddlewareBase(RequestDelegate next)
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

        public async Task InvokeAsyncBase(HttpContext context, TService service, string endpointRoute)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                var endpoint = context.Request.Path.Value;

                if (endpoint.EndsWith(endpointRoute))
                {
                    var requestModel = await DeserializeModel<TRequestModel>(context.Request);

                    var result = await ExecuteServiceMethod(requestModel, service, context);

                    if (result != null)
                    {
                        await Response(context, result);
                        return;
                    }

                }
            }

            await _next(context);
        }

        protected async Task Response(HttpContext context, TResponseModel result)
        {
            context.Response.ContentType = "application/json; charset=UTF-8";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result, _jsonSerializerSettings));
            await context.Response.Body.FlushAsync();
        }

        protected async Task<T> DeserializeModel<T>(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            {
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync(), _jsonSerializerSettings);
            }
        }

        protected abstract Task<TResponseModel> ExecuteServiceMethod(TRequestModel model, TService service, HttpContext context);
    }
}