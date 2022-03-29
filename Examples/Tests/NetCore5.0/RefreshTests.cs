using System.Net;
using System.Net.Http.Json;
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
        public async Task RefreshWithTheSameValidTokenMultipleTimes_ReturnsTokens()
        {
            var (_, authModel) = await LoginAsync(Login, Password, FingerPrint);

            await CallRefresh(authModel, FingerPrint);
            var (_, firstResult) = await CallRefresh(authModel, FingerPrint);

            await CallRefresh(authModel, FingerPrint);
            var (_, lastResult) = await CallRefresh(authModel, FingerPrint);

            Assert.False(string.IsNullOrWhiteSpace(lastResult.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(lastResult.RefreshToken.Value));

            Assert.Equal(lastResult.AccessToken.Value, firstResult.AccessToken.Value);
            Assert.Equal(lastResult.RefreshToken.Value, firstResult.RefreshToken.Value);
        }

        [Fact]
        public async Task RefreshWithInvalidToken_Returns401()
        {
            var (response, _) = await CallRefresh(new AuthResponseModel(), Guid.NewGuid().ToString());
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