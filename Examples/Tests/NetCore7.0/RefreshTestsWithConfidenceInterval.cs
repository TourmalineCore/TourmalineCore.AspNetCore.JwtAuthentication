using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.NetCore7._0
{
    public class RefreshTestsWithConfidenceInterval
        : AuthTestsBase<ProgramWithConfidenceInterval>
    {
        private const string Login = "Admin";
        private const string Password = "Admin";
        private const string FingerPrint = "fingerprint";

        private readonly AuthResponseModelEqualityComparer _authResponseModelEqualityComparer;

        public RefreshTestsWithConfidenceInterval(WebApplicationFactory<ProgramWithConfidenceInterval> factory)
            : base(factory)
        {
            _authResponseModelEqualityComparer = new AuthResponseModelEqualityComparer();
        }

        [Fact]
        public async Task RefreshWithTheSameValidTokenMultipleTimes_ReturnsTokens()
        {
            var (_, authModel) = await LoginAsync(Login, Password, FingerPrint);

            var (_, authResponseModel1) = await CallRefresh(authModel, FingerPrint);
            var (_, authResponseModel2) = await CallRefresh(authModel, FingerPrint);
            var (response, authResponseModel3) = await CallRefresh(authModel, FingerPrint);

            Assert.False(_authResponseModelEqualityComparer.Equals(authResponseModel1, authResponseModel2));
            Assert.False(_authResponseModelEqualityComparer.Equals(authResponseModel2, authResponseModel3));

            Assert.False(string.IsNullOrWhiteSpace(authResponseModel3.AccessToken.Value));
            Assert.False(string.IsNullOrWhiteSpace(authResponseModel3.RefreshToken.Value));

            Assert.True(response.IsSuccessStatusCode);
        }
    }
}