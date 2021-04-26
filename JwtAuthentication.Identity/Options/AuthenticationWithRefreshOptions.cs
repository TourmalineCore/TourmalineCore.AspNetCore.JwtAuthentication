using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class AuthenticationWithRefreshOptions : AuthenticationOptions
    {
        private int _refreshTokenExpireInMinutes;

        public int RefreshTokenExpireInMinutes
        {
            get => _refreshTokenExpireInMinutes == default ? 10080 : _refreshTokenExpireInMinutes;
            set => _refreshTokenExpireInMinutes = value;
        }
    }
}