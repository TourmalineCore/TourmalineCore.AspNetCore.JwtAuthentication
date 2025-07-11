using System.Net;
using System.Net.Http.Json;
using Example.Net5._0.RefreshTokenAuthAndRegistrationUsingIdentity;
using Microsoft.AspNetCore.Mvc.Testing;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;
using Xunit;

namespace Tests.Net5._0
{
    [Collection(nameof(Example.Net5._0.RefreshTokenAuthAndRegistrationUsingIdentity))]
    public class RefreshTests
        : AuthTestsBase<Startup>
    {
        private const string LogoutUrl = "/auth/logout";

        private const string Login = "Admin";
        private const string Password = "Admin";
        private const string FingerPrint = "fingerprint";

        public RefreshTests(WebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task RefreshWithValidToken_ReturnsTokens()
        {
            var loginResult = await LoginAsync(Login, Password);

            var (_, result) = await CallRefresh(loginResult.authModel);

            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken.Value));
        }

        [Fact]
        public async Task RefreshWithInvalidToken_Returns401()
        {
            var invalidAuthResponseModel = new AuthResponseModel
            {
                RefreshToken = new TokenModel
                {
                    Value = Guid.NewGuid().ToString(),
                },
                AccessToken = new TokenModel
                {
                    Value = string.Empty,
                },
            };

            var (response, _) = await CallRefresh(invalidAuthResponseModel, Guid.NewGuid().ToString());
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task RefreshWithTheSameValidTokenMultipleTimes_Returns401()
        {
            var (_, authModel) = await LoginAsync(Login, Password, FingerPrint);

            await CallRefresh(authModel, FingerPrint);
            var (response, _) = await CallRefresh(authModel, FingerPrint);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task LogoutWithValidToken_DiscardsRefresh()
        {
            var loginResult = await LoginAsync(Login, Password);

            var refreshResult = await CallRefresh(loginResult.authModel);

            var client = _factory.CreateClient();

            var body = JsonContent.Create(new RefreshTokenRequestModel
            {
                RefreshTokenValue = Guid.Parse(refreshResult.authModel.RefreshToken.Value),
            }
                );

            var logoutResult = await client.PostAsync(LogoutUrl, body);

            Assert.Equal(HttpStatusCode.OK, logoutResult.StatusCode);
        }
    }
}