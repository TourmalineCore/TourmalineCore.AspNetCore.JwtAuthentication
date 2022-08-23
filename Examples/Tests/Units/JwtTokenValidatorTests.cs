using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.TokenServices;
using Xunit;

namespace Tests.Units;

public class JwtTokenValidatorTests
{
    private readonly JwtTokenValidator _jwtTokenValidator;

    private const string PublicSigningKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsDwLnM5sbVi326YDsLvMkQLXDKVAaHrJZ/MwkoxF4Hmq4+pu4KojgQyVDtjseXG8UW5wbxW58eXG8V0XgJzsD8zQX2Z1bBawpIeD9sXf/5CFZGif85YFIqS3brqR3ScdGxYHXcwrUMGUCThxe918Q0aNXzdSxGGP2v7ZbtpFhLRyrTXHl4u6k3eyYG7zCkwextnMb9CJuCR7x1ua1V1S0xljAqg5PicFjt0vVSKzPM/Djw7XK84sJXxaet7t4cNtXVJIAyXUMsSli6gg9Cw9CEUSE40iWUR/6wrdUYAchk3vWiBhMmnufwzmFRLKHOH9Fz8buJVSrRfyt7a6S2iN+wIDAQABMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsDwLnM5sbVi326YDsLvMkQLXDKVAaHrJZ/MwkoxF4Hmq4+pu4KojgQyVDtjseXG8UW5wbxW58eXG8V0XgJzsD8zQX2Z1bBawpIeD9sXf/5CFZGif85YFIqS3brqR3ScdGxYHXcwrUMGUCThxe918Q0aNXzdSxGGP2v7ZbtpFhLRyrTXHl4u6k3eyYG7zCkwextnMb9CJuCR7x1ua1V1S0xljAqg5PicFjt0vVSKzPM/Djw7XK84sJXxaet7t4cNtXVJIAyXUMsSli6gg9Cw9CEUSE40iWUR/6wrdUYAchk3vWiBhMmnufwzmFRLKHOH9Fz8buJVSrRfyt7a6S2iN+wIDAQAB";
    private const string ValidJwtToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IkFkbWluIiwiZXhwIjoxOTc1OTIyNzg5fQ.oOU4-XE1dCyVtViamBU5BgLy9x24LrcAV4cTUB_rwgdzLvyVuL2FdvEIBAsDR-uZwzaWC4VOGWYxjOVgu-Qvr2Td4sx-6p5DLJhrycsPFSrs4qU62wC4qRtCA-7s-hrumVh_fxh04UCC00crXvZUXnyHfAxC4nKIKiNAaLZ_inM3I8TYRuyBtOt-m0-K17qs5n-NMh_7lw_nBFB76pi1LuOUW6WDGV1bVfzSVD1daCaUc33pX5RQGeYsE-BlHPgTp5trueVz79-Kn1MtXYx_Xf-ltr7wGKWy1dLNNJoZewqDq-I0zqApDfmaO6Gy8xgZAykBx529DGj5KlMLx29yoQ";
    private const string InvalidJwtToken = "abJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IkFkbWluIiwiZXhwIjoxOTc1OTIyNzg5fQ.oOU4-XE1dCyVtViamBU5BgLy9x24LrcAV4cTUB_rwgdzLvyVuL2FdvEIBAsDR-uZwzaWC4VOGWYxjOVgu-Qvr2Td4sx-6p5DLJhrycsPFSrs4qU62wC4qRtCA-7s-hrumVh_fxh04UCC00crXvZUXnyHfAxC4nKIKiNAaLZ_inM3I8TYRuyBtOt-m0-K17qs5n-NMh_7lw_nBFB76pi1LuOUW6WDGV1bVfzSVD1daCaUc33pX5RQGeYsE-BlHPgTp5trueVz79-Kn1MtXYx_Xf-ltr7wGKWy1dLNNJoZewqDq-I0zqApDfmaO6Gy8xgZAykBx529DGj5KlMLx29yoQ";

    public JwtTokenValidatorTests()
    {
        var authenticationOptions = new BaseAuthenticationOptions()
        {
            PublicSigningKey = PublicSigningKey
        };

        _jwtTokenValidator = new JwtTokenValidator(authenticationOptions);
    }

    [Fact]
    public async Task ValidateJwtToken_TokenIsValid_NoExceptions()
    {
        var exception = await Record.ExceptionAsync(() => _jwtTokenValidator.ValidateTokenAsync(ValidJwtToken));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateJwtToken_TokenIsInvalid_CatchExceptions()
    {
        var exception = await Record.ExceptionAsync(() => _jwtTokenValidator.ValidateTokenAsync(InvalidJwtToken));
        Assert.NotNull(exception);
    }
}