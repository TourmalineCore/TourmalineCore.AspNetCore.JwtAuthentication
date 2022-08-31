namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshAuthenticationOptions : AuthenticationOptions
    {
        private int _accessTokenExpireInMinutes;
        private int _refreshTokenExpireInMinutes;

        public int RefreshTokenExpireInMinutes
        {
            get => _refreshTokenExpireInMinutes == default ? 10080 : _refreshTokenExpireInMinutes;
            set => _refreshTokenExpireInMinutes = value;
        }

        public override int AccessTokenExpireInMinutes
        {
            get => _accessTokenExpireInMinutes == default ? 15 : _accessTokenExpireInMinutes;
            set => _accessTokenExpireInMinutes = value;
        }
    }
}