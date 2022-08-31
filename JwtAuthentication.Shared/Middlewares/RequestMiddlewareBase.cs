using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
#if NETCOREAPP3_0 || NETCOREAPP3_1
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#else
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares
{
    public abstract class RequestMiddlewareBase<TService, TRequestModel, TResponseModel>
    {
        private readonly RequestDelegate _next;
#if NETCOREAPP3_0 || NETCOREAPP3_1
        private readonly JsonSerializerSettings _jsonSerializerSettings;
#else
        private readonly JsonSerializerOptions _jsonSerializerSettings;
#endif
        protected RequestMiddlewareBase(RequestDelegate next)
        {
            _next = next;

#if NETCOREAPP3_0 || NETCOREAPP3_1
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
#else
            _jsonSerializerSettings = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                AllowTrailingCommas = true,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(),
                },
            };
#endif
        }

        protected async Task InvokeAsyncBase(HttpContext context, TService service, string endpointRoute)
        {
            try
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
            }
            catch (Exception ex)
            {
                var errorModelExample = new
                {
                    ErrorMessage = ex.Message,
                };

                await Response(context, errorModelExample);
                return;
            }            

            await _next(context);
        }

        private async Task Response<T>(HttpContext context, T result)
        {
            context.Response.ContentType = "application/json; charset=UTF-8";
#if NETCOREAPP3_0 || NETCOREAPP3_1
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result, _jsonSerializerSettings));
#else
            await context.Response.WriteAsync(JsonSerializer.Serialize(result, _jsonSerializerSettings));
#endif
            await context.Response.Body.FlushAsync();
        }

        private async Task<T> DeserializeModel<T>(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            {
#if NETCOREAPP3_0 || NETCOREAPP3_1
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync(), _jsonSerializerSettings);
#else
                return JsonSerializer.Deserialize<T>(await reader.ReadToEndAsync(), _jsonSerializerSettings);
#endif
            }
        }

        protected abstract Task<TResponseModel> ExecuteServiceMethod(TRequestModel requestModel, TService service, HttpContext context);
    }
}