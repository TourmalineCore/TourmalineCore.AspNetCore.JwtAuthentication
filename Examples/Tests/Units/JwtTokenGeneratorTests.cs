using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Signing;
using Xunit;

namespace Tests.Units;

public class JwtTokenGeneratorTests
{    
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private const string Issuer = null;
    private const string Audience = null;
    private SigningCredentials SigningCredentials = null;
    private DateTime Expires = new DateTime(2042, 12, 31);
    private List<Claim> EmptyClaims = new List<Claim>(); 

    private const string PrivateSigningKey = "MIIEowIBAAKCAQEAsDwLnM5sbVi326YDsLvMkQLXDKVAaHrJZ/MwkoxF4Hmq4+pu4KojgQyVDtjseXG8UW5wbxW58eXG8V0XgJzsD8zQX2Z1bBawpIeD9sXf/5CFZGif85YFIqS3brqR3ScdGxYHXcwrUMGUCThxe918Q0aNXzdSxGGP2v7ZbtpFhLRyrTXHl4u6k3eyYG7zCkwextnMb9CJuCR7x1ua1V1S0xljAqg5PicFjt0vVSKzPM/Djw7XK84sJXxaet7t4cNtXVJIAyXUMsSli6gg9Cw9CEUSE40iWUR/6wrdUYAchk3vWiBhMmnufwzmFRLKHOH9Fz8buJVSrRfyt7a6S2iN+wIDAQABAoIBAQCvue/KV3p+Pex2tD8RxvDf13kfPtfOVkDlyfQw7HXwsuDXijctBfmJAEbRGzQQlHw2pmyuF3fl4DxTB4Qb1lz8FDniJoQHV0ijhgzrz7rfVffsevajKH/OX3gYjShM4GeBTqHhwWefiqZV21YtMFhrrLniq4N4FeAfeebNRg/zlWEigraxqAWb4cplnxBE3qOBECKXdF/B8uhp743BU/2HLSO5BUdhtPlN3FKoYdyqtrKyNO2z7rC+Gk8tNd+KbMHDUMiOQXzbXkpsXYKAug9iTW+gxZG/bNyzGNrJBFrUYb1fP4iZphbxBJgobNYJBKA565cAX/wI5lFakTBB0YAhAoGBAOk0TyV0dA8WJ6NrWmRUBKsKvkSREhBveW+P3LtA8a1IgQf4K6ohIfcq9w/+nRvTLPIxo67FcqEyzVUu9TOafzIi59w4RBWG/HKOZ5lvIVicbuPyclPVWyC+9bMMgWEJy9wGwE+fGh3AvAA4PXNBcjOqfT0sSF9PBUo5qN11Q/qHAoGBAMF2IL+cXgPiUta4XoMh14ksJiwHtZeMkj+kauU3rctDITSkIGMFp4q0W5UUSG1yPcW/++rMQfuAjCZotdNpbQT+g+KfG44DMT5W7nRgv60S0/6X/OoLIhCue19yLMVzFpai0YEH+s24/XNnwl53K34G1zVMCsZcIuIng8SZVintAoGAJP/1pr2pRFOBin4X418pNnIH6h0SPqVRIRA0N0mAjru4LSmE1ANZvjuE43bEOovwz6Rskegl3cmPpnpC0SMsFypOmzQaKUg3eX16lm95XPPE7EmlNgPd534kwXm0dU72lzxC+t8FZ78SlP5XUZgKpIPiRvhlqymAb1xinHBkjrUCgYAB144YRPTgNJd1U+wSc5AJzlHOuYQRHVWHJZme9RjChrEaPzXPu44M1ArLMJY/9IaCC4HqimdWbbLn6rdQfAB9u66lyb4JbB5b6Zf7o7Avha5fDjNqRxDb981U61Fhz+a3KHW2NM0+iDRhlOtU2u2fFZGXAFJZ8Saj4JxwksUvQQKBgEQ1TAW/INhWSkEW8vGeLnjV+rxOx8EJ9ftVCRaQMlDEDlX0n7BZeQrQ1pBxwL0FSTrUQdD02MsWshrhe0agKsw2Yaxn8gYs1v9HMloS4Q3L2zl8pi7R3yx72RIcdnS4rqGXeO5t8dm305Yz2RHhqtkBmpFBssSEYCY/tUDmsQVU";

    public JwtTokenGeneratorTests()
    {
        _jwtTokenGenerator = new JwtTokenGenerator();

        var privateKey = SigningHelper.GetPrivateKey(PrivateSigningKey);
        SigningCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
    }

    [Fact]
    public void GenerateJwtToken_TokenTypeIsInvalid_CatchException()
    {
        var exception = Record.Exception(() => _jwtTokenGenerator.Generate(Issuer, Audience, EmptyClaims, Expires, SigningCredentials, "invalidTokenValue"));
        Assert.NotNull(exception);
    }

    [Fact]
    public void GenerateAccessJwtToken()
    {
        var expectedAccessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlblR5cGUiOiJhY2Nlc3MiLCJleHAiOjIzMDM1Nzg4MDB9.X89rNuFFSFpwnwJ1j0bg89XPBfD4qkxk_h76IO3ogThtHWT2S5bnfUmQM-as1zevC4CSljIUWl4TNzg-R9YNPqkZVs7yeztP4V9nj2SCRH5LllPbL0ULsuxxkGKpbpPN7bNEXTiykbwE7Ea42aYR1qjrZABvWn2WBI3dsVfzRqXEvsUMKq-8pxl8K45JLdV87bkO3uy1PWhadb9E_qPM2f6Y1xRXPR5y685pDmRZYJAqSbId_uKN52CzE2eiwQXxD3jyXadMLsIwS489XQ01k3HGWvpH-0ilUr_hhyPGNGL9_NJc5SqkDADqxh9doLqyHo3cPM4MZ6FilI4qpCciJg";
        var accessToken = _jwtTokenGenerator.Generate(Issuer, Audience, EmptyClaims, Expires, SigningCredentials, TokenType.Access);

        Assert.Equal(expectedAccessToken, accessToken);
        Assert.Equal(TokenType.Access, GetTokenTypeClaim(accessToken).Value);
    }

    [Fact]
    public void GenerateRefreshJwtToken()
    {
        var expectedAccessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlblR5cGUiOiJyZWZyZXNoIiwiZXhwIjoyMzAzNTc4ODAwfQ.goyF9EVfnq0np_r473GICCfJDKy8-7QGe1UV4Ara2qolKMRa7mXAgyoI6d1RQhO7NhnfSbsO0dlISsoNgZ0Xh72K253DOf6MFuri84uPSf1-Q5zqBz4z97s1RjpEWcPt-eirXs0uhaZ1Uyf0rYccUBst6ORnfUHRXyqTtujenKybFCokY4bRmCnjIvfdK-QVZlmF3npKZzcb0A6lrPfyaBS45G2iN8Gw5AjCikPfKyKguBFNIb8JpFaOgCKBaE-d6HrSrJGKkBiE3m0jhTelEN9qADwDefAA15iBTLBK_CbNbzyWuL47fuvguWotozBU2OyD-9lqSo4Fpxw9VyhTOw";
        var accessToken = _jwtTokenGenerator.Generate(Issuer, Audience, EmptyClaims, Expires, SigningCredentials, TokenType.Refresh);

        Assert.Equal(expectedAccessToken, accessToken);
        Assert.Equal(TokenType.Refresh, GetTokenTypeClaim(accessToken).Value);
    }

    [Fact]
    public void GenerateAccessJwtToken_TokenTypeClaimAlreadyAdded()
    {
        var expectedAccessToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlblR5cGUiOiJhY2Nlc3MiLCJleHAiOjIzMDM1Nzg4MDB9.X89rNuFFSFpwnwJ1j0bg89XPBfD4qkxk_h76IO3ogThtHWT2S5bnfUmQM-as1zevC4CSljIUWl4TNzg-R9YNPqkZVs7yeztP4V9nj2SCRH5LllPbL0ULsuxxkGKpbpPN7bNEXTiykbwE7Ea42aYR1qjrZABvWn2WBI3dsVfzRqXEvsUMKq-8pxl8K45JLdV87bkO3uy1PWhadb9E_qPM2f6Y1xRXPR5y685pDmRZYJAqSbId_uKN52CzE2eiwQXxD3jyXadMLsIwS489XQ01k3HGWvpH-0ilUr_hhyPGNGL9_NJc5SqkDADqxh9doLqyHo3cPM4MZ6FilI4qpCciJg";

        var claims = new List<Claim>()
        {
            new Claim(Consts.TokenTypeClaimName, TokenType.Access)
        };

        var accessToken = _jwtTokenGenerator.Generate(Issuer, Audience, claims, Expires, SigningCredentials, TokenType.Access);

        Assert.Equal(expectedAccessToken, accessToken);
        Assert.Equal(TokenType.Access, GetTokenTypeClaim(accessToken).Value);
    }

    [Fact]
    public void GenerateAccessJwtToken_IfThereAreAdditionalClaims()
    {
        const string additionalClaimType = "ExamplePermission";
        const string additionalClaimValue = "ExampleClaimValue";

        var claims = new List<Claim>()
        {
            new Claim(additionalClaimType, additionalClaimValue),
        };

        var accessToken = _jwtTokenGenerator.Generate(Issuer, Audience, claims, Expires, SigningCredentials, TokenType.Access);

        var exampleClaim = GetClaim(accessToken, additionalClaimType);
        var tokenTypeClaim = GetTokenTypeClaim(accessToken);

        Assert.Equal(additionalClaimValue, exampleClaim.Value);
        Assert.Equal(TokenType.Access, tokenTypeClaim.Value);
    }

    private Claim GetClaim(string tokenValue, string claimType)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);

        return token.Claims.SingleOrDefault(claim => claim.Type == claimType);
    }

    private Claim GetTokenTypeClaim(string tokenValue)
    {
        return GetClaim(tokenValue, Consts.TokenTypeClaimName);
    }
}