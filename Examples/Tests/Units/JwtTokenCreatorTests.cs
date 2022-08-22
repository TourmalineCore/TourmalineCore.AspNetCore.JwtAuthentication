using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices.Contracts;
using Xunit;

namespace Tests.Units;

public class JwtTokenCreatorTests
{    
    private readonly IJwtTokenCreator _jwtTokenCreator;
    private const int TokenLifetimeInMinutes = 15;

    public JwtTokenCreatorTests()
    {
        var authenticationOptions = new SharedAuthenticationOptions()
        {
            PrivateSigningKey = "MIIEowIBAAKCAQEAsDwLnM5sbVi326YDsLvMkQLXDKVAaHrJZ/MwkoxF4Hmq4+pu4KojgQyVDtjseXG8UW5wbxW58eXG8V0XgJzsD8zQX2Z1bBawpIeD9sXf/5CFZGif85YFIqS3brqR3ScdGxYHXcwrUMGUCThxe918Q0aNXzdSxGGP2v7ZbtpFhLRyrTXHl4u6k3eyYG7zCkwextnMb9CJuCR7x1ua1V1S0xljAqg5PicFjt0vVSKzPM/Djw7XK84sJXxaet7t4cNtXVJIAyXUMsSli6gg9Cw9CEUSE40iWUR/6wrdUYAchk3vWiBhMmnufwzmFRLKHOH9Fz8buJVSrRfyt7a6S2iN+wIDAQABAoIBAQCvue/KV3p+Pex2tD8RxvDf13kfPtfOVkDlyfQw7HXwsuDXijctBfmJAEbRGzQQlHw2pmyuF3fl4DxTB4Qb1lz8FDniJoQHV0ijhgzrz7rfVffsevajKH/OX3gYjShM4GeBTqHhwWefiqZV21YtMFhrrLniq4N4FeAfeebNRg/zlWEigraxqAWb4cplnxBE3qOBECKXdF/B8uhp743BU/2HLSO5BUdhtPlN3FKoYdyqtrKyNO2z7rC+Gk8tNd+KbMHDUMiOQXzbXkpsXYKAug9iTW+gxZG/bNyzGNrJBFrUYb1fP4iZphbxBJgobNYJBKA565cAX/wI5lFakTBB0YAhAoGBAOk0TyV0dA8WJ6NrWmRUBKsKvkSREhBveW+P3LtA8a1IgQf4K6ohIfcq9w/+nRvTLPIxo67FcqEyzVUu9TOafzIi59w4RBWG/HKOZ5lvIVicbuPyclPVWyC+9bMMgWEJy9wGwE+fGh3AvAA4PXNBcjOqfT0sSF9PBUo5qN11Q/qHAoGBAMF2IL+cXgPiUta4XoMh14ksJiwHtZeMkj+kauU3rctDITSkIGMFp4q0W5UUSG1yPcW/++rMQfuAjCZotdNpbQT+g+KfG44DMT5W7nRgv60S0/6X/OoLIhCue19yLMVzFpai0YEH+s24/XNnwl53K34G1zVMCsZcIuIng8SZVintAoGAJP/1pr2pRFOBin4X418pNnIH6h0SPqVRIRA0N0mAjru4LSmE1ANZvjuE43bEOovwz6Rskegl3cmPpnpC0SMsFypOmzQaKUg3eX16lm95XPPE7EmlNgPd534kwXm0dU72lzxC+t8FZ78SlP5XUZgKpIPiRvhlqymAb1xinHBkjrUCgYAB144YRPTgNJd1U+wSc5AJzlHOuYQRHVWHJZme9RjChrEaPzXPu44M1ArLMJY/9IaCC4HqimdWbbLn6rdQfAB9u66lyb4JbB5b6Zf7o7Avha5fDjNqRxDb981U61Fhz+a3KHW2NM0+iDRhlOtU2u2fFZGXAFJZ8Saj4JxwksUvQQKBgEQ1TAW/INhWSkEW8vGeLnjV+rxOx8EJ9ftVCRaQMlDEDlX0n7BZeQrQ1pBxwL0FSTrUQdD02MsWshrhe0agKsw2Yaxn8gYs1v9HMloS4Q3L2zl8pi7R3yx72RIcdnS4rqGXeO5t8dm305Yz2RHhqtkBmpFBssSEYCY/tUDmsQVU",
        };

        _jwtTokenCreator = new JwtTokenCreator(authenticationOptions);
    }

    [Fact]
    public async Task CreateJwtToken_TokenTypeIsInvalid_CatchException()
    {
        var exception = await Record.ExceptionAsync(() => _jwtTokenCreator.CreateAsync("invalidTokenType", TokenLifetimeInMinutes));
        Assert.NotNull(exception);
        Assert.Equal("Invalid token type value", exception.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task CreateJwtToken_TokenLifetimeIsInvalid_CatchException(int tokenLifetimeInMinutes)
    {
        var exception = await Record.ExceptionAsync(() => _jwtTokenCreator.CreateAsync(TokenType.Access, tokenLifetimeInMinutes));
        Assert.NotNull(exception);
        Assert.Equal("Token lifetime cannot be negative or zero", exception.Message);
    }

    [Fact]
    public async Task CreateAccessJwtToken()
    {
        var accessTokenModel = await _jwtTokenCreator.CreateAsync(TokenType.Access, TokenLifetimeInMinutes);
        Assert.Equal(TokenType.Access, GetTokenTypeClaim(accessTokenModel).Value);
    }

    [Fact]
    public async Task CreateRefreshJwtToken()
    {
        var refreshTokenModel = await _jwtTokenCreator.CreateAsync(TokenType.Refresh, TokenLifetimeInMinutes);
        Assert.Equal(TokenType.Refresh, GetTokenTypeClaim(refreshTokenModel).Value);
    }

    [Fact]
    public async Task CreateAccessJwtToken_TokenTypeClaimAlreadyAdded()
    {
        var claims = new List<Claim>()
        {
            new Claim(Consts.TokenTypeClaimName, TokenType.Access)
        };

        var accessTokenModel = await _jwtTokenCreator.CreateAsync(TokenType.Access, TokenLifetimeInMinutes, claims);
        Assert.Equal(TokenType.Access, GetTokenTypeClaim(accessTokenModel).Value);
    }

    [Fact]
    public async Task CreateAccessJwtToken_IfThereAreAdditionalClaims()
    {
        const string additionalClaimType = "ExamplePermission";
        const string additionalClaimValue = "ExampleClaimValue";

        var claims = new List<Claim>()
        {
            new Claim(additionalClaimType, additionalClaimValue),
        };

        var accessTokenModel = await _jwtTokenCreator.CreateAsync(TokenType.Access, TokenLifetimeInMinutes, claims);

        var exampleClaim = GetClaim(accessTokenModel.Value, additionalClaimType);
        var tokenTypeClaim = GetTokenTypeClaim(accessTokenModel);

        Assert.Equal(additionalClaimValue, exampleClaim.Value);
        Assert.Equal(TokenType.Access, tokenTypeClaim.Value);
    }

    private Claim GetClaim(string tokenValue, string claimType)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);

        return token.Claims.SingleOrDefault(claim => claim.Type == claimType);
    }

    private Claim GetTokenTypeClaim(TokenModel tokenModel)
    {
        return GetClaim(tokenModel.Value, Consts.TokenTypeClaimName);
    }
}