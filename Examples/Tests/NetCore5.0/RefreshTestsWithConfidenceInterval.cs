using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.NetCore5._0
{
    [Collection(nameof(Example.NetCore5._0.RefreshTokenWithConfidenceInterval))]
    public class RefreshTestsWithConfidenceInterval
        : AuthTestsBase<ProgramWithConfidenceInterval>
    {
        private const string Login = "Admin";
        private const string Password = "Admin";
        private const string FingerPrint = "fingerprint";

        public RefreshTestsWithConfidenceInterval(WebApplicationFactory<ProgramWithConfidenceInterval> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task RefreshWithTheSameValidTokenMultipleTimes_ReturnsTokens()
        {
            var (_, authModel) = await LoginAsync(Login, Password, FingerPrint);

            await CallRefresh(authModel, FingerPrint);
            await CallRefresh(authModel, FingerPrint);

            var (response, result) = await CallRefresh(authModel, FingerPrint);

            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken.Value));

            Assert.True(response.IsSuccessStatusCode);
        }
    }
}