namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    internal interface IJwtTokenValidator
    {
        void Validate(string jwtToken);
    }
}
