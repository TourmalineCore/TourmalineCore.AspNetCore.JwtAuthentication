using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using Xunit;

namespace Tests.NetCore8._0
{
    public class BasicAuthTestWithGeneric :
    IClassFixture<WebApplicationFactory<ProgramGeneric>>
    {
        protected readonly WebApplicationFactory<ProgramGeneric> _factory;
        protected readonly JsonSerializerOptions _jsonSerializerSettings;

        protected const string LoginUrl = "/auth/login";
        protected const string AuthorizedEndpointUrl = "/example";

        private const string Login = "Admin";
        private const string Password = "Admin";

        public BasicAuthTestWithGeneric(WebApplicationFactory<ProgramGeneric> factory)
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

        [Fact]
        public async Task LoginWithValidCreds_ReturnsTokens()
        {
            var (_, authModel) = await LoginAsync(Login, Password);

            Assert.False(string.IsNullOrWhiteSpace(authModel.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(authModel.RefreshToken.Value));
        }

        [Fact]
        public async Task CallAuthorizedEndpointWithInvalidToken_Returns401()
        {
            var response = await CallAuthorizedEndpointAsync("invalid");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CallAuthorizedEndpointWithValidToken_Returns200()
        {
            var (_, authModel) = await LoginAsync(Login, Password);
            var response = await CallAuthorizedEndpointAsync(authModel.AccessToken.Value);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginWithInvalidCreds_Returns401()
        {
            var (response, _) = await LoginAsync(Login, "123");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TwoSuccessfulLoginsInARow_ReturnsTokens()
        {
            var (_, firstAuthModel) = await LoginAsync(Login, Password);
            var (_, secondAuthModel) = await LoginAsync(Login, Password);

            Assert.False(string.IsNullOrWhiteSpace(firstAuthModel.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(secondAuthModel.AccessToken.Value));
            Assert.NotEqual(firstAuthModel.RefreshToken.Value, secondAuthModel.RefreshToken.Value);
        }

        internal async Task<(HttpResponseMessage response, AuthResponseModel authModel)> LoginAsync(string login, string password)
        {
            var client = _factory.CreateClient();

            var body = JsonContent.Create(new LoginRequestModel
            {
                Login = login,
                Password = password,
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