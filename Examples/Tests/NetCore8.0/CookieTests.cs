using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Net8._0
{
    public class CookieTests
        : AuthTestsBase<ProgramCookies>
    {
        private const string Login = "Admin";
        private const string Password = "Admin";

        public CookieTests(WebApplicationFactory<ProgramCookies> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task LoginWithValidCreds_SetsValidCookieInResponse()
        {
            var (loginResponse, _) = await LoginAsync(Login, Password);

            var cookies = loginResponse
                .Headers
                .SingleOrDefault(header => header.Key == "Set-Cookie")
                .Value;

            var client = _factory.CreateClient();

            var message = new HttpRequestMessage(HttpMethod.Get, AuthorizedEndpointUrl);
            message.Headers.Add("Cookie", cookies);

            var response = await client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LoginWithInvalidCreds_DoesNotSetCookieInResponse()
        {
            var (loginResponse, _) = await LoginAsync(Login, "123");

            var cookies = loginResponse
                .Headers
                .SingleOrDefault(header => header.Key == "Set-Cookie");

            Assert.Null(cookies.Value);
        }

        [Fact]
        public async Task UsingInvalidCookie_Returns401()
        {
            var client = _factory.CreateClient();

            var message = new HttpRequestMessage(HttpMethod.Get, AuthorizedEndpointUrl);
            message.Headers.Add("Cookie", new[] { "invalid-cookie" });

            var response = await client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}