using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity;
using Microsoft.AspNetCore.Mvc.Testing;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using Xunit;

namespace Tests.NetCore5._0
{
    [Collection(nameof(Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity))]
    public class LogoutTest
        : AuthTestsBase<Startup>
    {
        private const string LogoutUrl = "/auth/logout";

        private const string Login = "Admin";
        private const string Password = "Admin";

        public LogoutTest(WebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task LogoutWithValidToken_Return200()
        {
            var loginResult = await LoginAsync(Login, Password);

            var client = _factory.CreateClient();

            var body = JsonContent.Create(new RefreshTokenRequestModel
                    {
                        RefreshTokenValue = Guid.Parse(loginResult.authModel.RefreshToken.Value),
                    }
                );

            var logoutResult = await client.PostAsync(LogoutUrl, body);

            Assert.Equal(HttpStatusCode.OK, logoutResult.StatusCode);
        }

        [Fact]
        public async Task LogoutWithInvalidToken_Return400()
        {
            var client = _factory.CreateClient();

            var body = JsonContent.Create(new RefreshTokenRequestModel
                    {
                        RefreshTokenValue = Guid.NewGuid(),
                    }
                );

            var logoutResult = await client.PostAsync(LogoutUrl, body);

            Assert.Equal(HttpStatusCode.InternalServerError, logoutResult.StatusCode);
        }
    }
}