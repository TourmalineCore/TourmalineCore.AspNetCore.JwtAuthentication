using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity;
using Microsoft.AspNetCore.Mvc.Testing;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using Xunit;

namespace Tests.NetCore5._0
{
    [Collection(nameof(Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity))]
    public class RegistrationTests
        : AuthTestsBase<Startup>
    {
        private const string RegisterUrl = "/auth/register";

        private const string Login = "test";
        private const string Password = "Test1234.";

        public RegistrationTests(WebApplicationFactory<Startup> factory)
            :base(factory)
        {
        }

        [Fact]
        public async Task RegisterWithInvalidCreds_Returns400()
        {
            var registerResult = await RegisterAsync(Login, "1");
            Assert.Equal(HttpStatusCode.BadRequest, registerResult.response.StatusCode);
        }

        [Fact]
        public async Task RegisterWithValidCreds_CreatesNewUser()
        {
            var registerResult = await RegisterAsync(Login, Password);

            var callAuthorizedEndpointResult = await CallAuthorizedEndpointAsync(registerResult.authModel.AccessToken.Value);
            Assert.Equal(HttpStatusCode.OK, callAuthorizedEndpointResult.StatusCode);

            var loginResult = await LoginAsync(Login, Password);

            Assert.False(string.IsNullOrWhiteSpace(loginResult.authModel.AccessToken.Value));
        }

        [Fact]
        public async Task RegisterWithCredsOfExistingUser_Returns400()
        {
            var registerResult = await RegisterAsync("Admin", "Admin");
            Assert.Equal(HttpStatusCode.BadRequest, registerResult.response.StatusCode);
        }

        private async Task<(HttpResponseMessage response, AuthResponseModel authModel)> RegisterAsync(string login, string password)
        {
            var client = _factory.CreateClient();

            var body = JsonContent.Create(new RegistrationRequestModel()
                    {
                        Login = login,
                        Password = password,
                    }
                );

            var response = await client.PostAsync(RegisterUrl, body);
            var result = JsonSerializer.Deserialize<AuthResponseModel>(response.Content.ReadAsStringAsync().Result, _jsonSerializerSettings);
            return (response, result);
        }
    }
}