using System.Net;
using System.Threading.Tasks;
using Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.NetCore5._0
{
    [Collection(nameof(Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity))]
    public class BasicAuthTests
        : AuthTestsBase<Startup>
    {
        private const string Login = "Admin";
        private const string Password = "Admin";

        public BasicAuthTests(WebApplicationFactory<Startup> factory)
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
    }
}