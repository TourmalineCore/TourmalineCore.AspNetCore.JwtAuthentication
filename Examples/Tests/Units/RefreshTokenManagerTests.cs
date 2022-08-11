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
    private readonly Guid _refreshTokenGuid1 = Guid.NewGuid();
    private readonly Guid _refreshTokenGuid2 = Guid.NewGuid();

    public RefreshTokenManagerTests()
    {
        var authenticationOptions = new RefreshAuthenticationOptions();

        _refreshTokenManager = new RefreshTokenManager<CustomUser, string>(GetDbContextMock().Object, authenticationOptions);
    }

    [Fact]
    public async Task IsRefreshTokenInConfidenceInterval_TokenExpirationTimeIsInConfidenceInterval_ReturnTrue()
    {
        var refreshTokenIsInConfidenceInterval = await _refreshTokenManager.IsRefreshTokenInConfidenceIntervalAsync("1", _refreshTokenGuid1, RefreshConfidenceIntervalInMilliseconds);

        Assert.True(refreshTokenIsInConfidenceInterval);
    }

    [Fact]
    public async Task IsRefreshTokenInConfidenceInterval_TokenExpirationTimeIsNotInConfidenceInterval_ReturnFalse()
    {
        var refreshTokenIsInConfidenceInterval = await _refreshTokenManager.IsRefreshTokenInConfidenceIntervalAsync("2", _refreshTokenGuid2, RefreshConfidenceIntervalInMilliseconds);

        Assert.False(refreshTokenIsInConfidenceInterval);
    }

    private Mock<TourmalineDbContext<CustomUser>> GetDbContextMock()
    {
        var tokens = new List<RefreshToken<CustomUser, string>>
        {
            new()
            {
                ExpiredAtUtc = DateTime.UtcNow,
                ExpiresIn = DateTime.UtcNow + TimeSpan.FromDays(7),
                Value = _refreshTokenGuid1,
                UserId = "1",
                User = new CustomUser
                {
                    Id = "1",
                },
            },
            new()
            {
                ExpiredAtUtc = new DateTime(2021, 01, 01),
                ExpiresIn = DateTime.UtcNow + TimeSpan.FromDays(7),
                Value = _refreshTokenGuid2,
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