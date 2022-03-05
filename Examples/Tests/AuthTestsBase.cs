using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using Xunit;

namespace Tests
{
    public class AuthTestsBase<TProject>
        : IClassFixture<WebApplicationFactory<TProject>> where TProject : class
    {
        protected readonly WebApplicationFactory<TProject> _factory;
        protected readonly JsonSerializerOptions _jsonSerializerSettings;

        protected const string LoginUrl = "/auth/login";
        protected const string AuthorizedEndpointUrl = "/example";

        public AuthTestsBase(WebApplicationFactory<TProject> factory)
        {
            _factory = factory;

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
        }

        internal async Task<(HttpResponseMessage response, AuthResponseModel authModel)> LoginAsync(string login, string password, string clientFingerPrint = null)
        {
            var client = _factory.CreateClient();

            var body = JsonContent.Create(new LoginRequestModel
                    {
                        Login = login,
                        Password = password,
                        ClientFingerPrint = clientFingerPrint,
                    }
                );

            var response = await client.PostAsync(LoginUrl, body);

            var authModel = JsonSerializer.Deserialize<AuthResponseModel>(response.Content.ReadAsStringAsync().Result, _jsonSerializerSettings);

            return (response, authModel);
        }

        internal async Task<HttpResponseMessage> CallAuthorizedEndpointAsync(string accessToken)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync(AuthorizedEndpointUrl);
            return response;
        }
    }
}