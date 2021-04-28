using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware
{
    internal class RegistrationMiddleware<TUser, TRegistrationRequestModel> 
        where TUser : IdentityUser 
        where TRegistrationRequestModel : RegistrationRequestModel
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly RegistrationOptions _options;
        private readonly Func<TRegistrationRequestModel, TUser> _mapping;
        private readonly RequestDelegate _next;

        public RegistrationMiddleware(
            RequestDelegate next,
            Func<TRegistrationRequestModel, TUser> mapping,
            RegistrationOptions options = null
            )
        {
            _mapping = mapping;
            _options = options;
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

        public async Task InvokeAsync(HttpContext context, IRegistrationService<TUser, TRegistrationRequestModel> registrationService)
        {
            if (context.Request.Method == HttpMethods.Post)
            {
                var endpoint = context.Request.Path.Value;

                if (endpoint.EndsWith(_options.RegistrationEndpointRoute))
                {
                    try
                    {
                        var requestModel = await DeserializeModel<TRegistrationRequestModel>(context.Request);
                        var result = await registrationService.RegisterAsync(requestModel, _mapping);

                        if (result != null)
                        {
                            await Response(context, result);
                        }

                    }
                    catch (RegistrationException)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
            }

            await _next(context);
        }

        private async Task Response(HttpContext context, AuthResponseModel result)
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
    }
}