using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

namespace Tests;

internal class AuthResponseModelEqualityComparer : IEqualityComparer<AuthResponseModel>
{
    public bool Equals(AuthResponseModel? x, AuthResponseModel? y)
    {
        return x.AccessToken == y.AccessToken && x.RefreshToken == y.RefreshToken;
    }

    public int GetHashCode(AuthResponseModel obj)
    {
        return obj.AccessToken.GetHashCode() + obj.RefreshToken.GetHashCode();
    }
}