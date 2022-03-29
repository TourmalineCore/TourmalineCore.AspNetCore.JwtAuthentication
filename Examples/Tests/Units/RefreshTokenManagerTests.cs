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
    private readonly RefreshTokenManager<CustomUser, string> _refreshTokenManager;
    private readonly Guid _guidForTokenWhichProbablyHasStolen = Guid.NewGuid();
    private readonly Guid _guidForTokenWhichRecentlyExpired = Guid.NewGuid();

    public RefreshTokenManagerTests()
    {
        var authenticationOptions = new RefreshAuthenticationOptions();

        _refreshTokenManager = new RefreshTokenManager<CustomUser, string>(GetDbContextMock().Object, authenticationOptions);
    }

    [Fact]
    public async Task CheckTokenOnPotentialTheft_TokenExpiredLongTimeAgo_ReturnTrue()
    {
        var res = await _refreshTokenManager.IsPotentialRefreshTokenTheft("1", _guidForTokenWhichProbablyHasStolen);

        Assert.True(res);
    }

    [Fact]
    public async Task CheckTokenOnPotentialTheft_TokenExpiredRecently_ReturnFalse()
    {
        var res = await _refreshTokenManager.IsPotentialRefreshTokenTheft("2", _guidForTokenWhichRecentlyExpired);

        Assert.False(res);
    }

    private Mock<TourmalineDbContext<CustomUser>> GetDbContextMock()
    {
        var tokens = new List<RefreshToken<CustomUser>>
        {
            new()
            {
                ExpiredAt = new DateTime(2021, 01, 01),
                ExpiresIn = DateTime.UtcNow + TimeSpan.FromDays(7),
                Value = _guidForTokenWhichProbablyHasStolen,
                UserId = "1",
            },
            new()
            {
                ExpiredAt = DateTime.UtcNow,
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
            .Setup(x => x.Set<RefreshToken<CustomUser>>())
            .Returns(tokens.AsQueryable().BuildMockDbSet().Object);

        return appDbContextMock;
    }
}