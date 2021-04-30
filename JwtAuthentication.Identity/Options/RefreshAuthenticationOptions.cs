using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshAuthenticationOptions : AuthenticationOptions
    {
        private int _refreshTokenExpireInMinutes;

        public int RefreshTokenExpireInMinutes
        {
            get => _refreshTokenExpireInMinutes == default ? 10080 : _refreshTokenExpireInMinutes;
            set => _refreshTokenExpireInMinutes = value;
        }

        public override int AccessTokenExpireInMinutes
        {
            get => base.AccessTokenExpireInMinutes == default ? 15 : base.AccessTokenExpireInMinutes;
            set => base.AccessTokenExpireInMinutes = value;
        }
    }
}