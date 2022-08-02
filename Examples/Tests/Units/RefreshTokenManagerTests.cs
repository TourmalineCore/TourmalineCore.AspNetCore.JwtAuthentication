using Example.NetCore6._0.RefreshTokenAuthAndRegistrationUsingIdentity.Models;
using MockQueryable.Moq;
using Moq;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Services;
using Xunit;

namespace Tests.Units;

public class RefreshTokenManagerTests
{
    private const int RefreshConfidenceIntervalInMilliseconds = 300_000;

    private readonly RefreshTokenManager<CustomUser, string> _refreshTokenManager;
    private readonly Guid _guidForTokenWhichProbablyHasStolen = Guid.NewGuid();
    private readonly Guid _guidForTokenWhichRecentlyExpired = Guid.NewGuid();

    public RefreshTokenManagerTests()
    {
        var authenticationOptions = new RefreshAuthenticationOptions();

        _refreshTokenManager = new RefreshTokenManager<CustomUser, string>(GetDbContextMock().Object, authenticationOptions);
    }

    [Fact]
    public async Task CheckTokenWithRefreshConfidenceInterval_TokenExpiredLongTimeAgo_ReturnTrue()
    {
        var res = await _refreshTokenManager.IsRefreshTokenSuspiciousAsync("1", _guidForTokenWhichProbablyHasStolen, RefreshConfidenceIntervalInMilliseconds);

        Assert.True(res);
    }

    [Fact]
    public async Task CheckTokenWithRefreshConfidenceInterval_TokenExpiredRecently_ReturnFalse()
    {
        var res = await _refreshTokenManager.IsRefreshTokenSuspiciousAsync("2", _guidForTokenWhichRecentlyExpired, RefreshConfidenceIntervalInMilliseconds);

        Assert.False(res);
    }

    private Mock<TourmalineDbContext<CustomUser>> GetDbContextMock()
    {
        var tokens = new List<RefreshToken<CustomUser, string>>
        {
            new()
            {
                ExpiredAtUtc = new DateTime(2021, 01, 01),
                ExpiresIn = DateTime.UtcNow + TimeSpan.FromDays(7),
                Value = _guidForTokenWhichProbablyHasStolen,
                UserId = "1",
                User = new CustomUser
                {
                    Id = "1",
                },
            },
            new()
            {
                ExpiredAtUtc = DateTime.UtcNow,
                ExpiresIn = DateTime.UtcNow + TimeSpan.FromDays(7),
                Value = _guidForTokenWhichRecentlyExpired,
                UserId = "2",
                User = new CustomUser
                {
                    Id = "2",
                },
            },
        };

        var appDbContextMock = new Mock<TourmalineDbContext<CustomUser>>();

        appDbContextMock
            .Setup(x => x.Set<RefreshToken<CustomUser, string>>())
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        return appDbContextMock;
    }
}