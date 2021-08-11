using System;
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
    public class RefreshTests
        : AuthTestsBase<Startup>
    {
        private const string RefreshUrl = "/auth/refresh";
        private const string LogoutUrl = "/auth/logout";

        private const string Login = "Admin";
        private const string Password = "Admin";

        public RefreshTests(WebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task RefreshWithValidToken_ReturnsTokens()
        {
            var loginResult = await LoginAsync(Login, Password);

            var (_, result) = await CallRefresh(loginResult.authModel.RefreshToken.Value);

            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken.Value));
        }

        [Fact]
        public async Task RefreshWithInvalidToken_Returns401()
        {
            var (response, _) = await CallRefresh(Guid.NewGuid().ToString());
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task LogoutWithValidToken_DiscardsRefresh()
        {
            var loginResult = await LoginAsync(Login, Password);

            var refreshResult = await CallRefresh(loginResult.authModel.RefreshToken.Value);

            var client = _factory.CreateClient();

            var body = JsonContent.Create(new RefreshTokenRequestModel
                    {
                        RefreshTokenValue = Guid.Parse(refreshResult.authModel.RefreshToken.Value),
                    }
                );

            var logoutResult = await client.PostAsync(LogoutUrl, body);

            Assert.Equal(HttpStatusCode.OK, logoutResult.StatusCode);

            var secondRefreshResult = await CallRefresh(refreshResult.authModel.RefreshToken.Value);
            Assert.Equal(HttpStatusCode.Conflict, secondRefreshResult.response.StatusCode);
        }

        private async Task<(HttpResponseMessage response, AuthResponseModel authModel)> CallRefresh(string refresh, string fingerprint = null)
        {
            var client = _factory.CreateClient();

            var body = JsonContent.Create(new RefreshTokenRequestModel
                    {
                        RefreshTokenValue = Guid.Parse(refresh),
                        ClientFingerPrint = fingerprint,
                    }
                );

            var response = await client.PostAsync(RefreshUrl, body);
            var result = JsonSerializer.Deserialize<AuthResponseModel>(response.Content.ReadAsStringAsync().Result, _jsonSerializerSettings);
            return (response, result);
        }
    }
}