using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Net6._0
{
    [Collection(nameof(Example.Net6._0.RefreshTokenAuthAndRegistrationUsingIdentity))]
    public class BasicAuthTests
        : AuthTestsBase<Program>
    {
        private const string Login = "Admin";
        private const string Password = "Admin";

        public BasicAuthTests(WebApplicationFactory<Program> factory)
            : base(factory)
        {
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
    }
}