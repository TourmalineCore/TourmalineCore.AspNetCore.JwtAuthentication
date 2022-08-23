namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options
{
    public class BaseRefreshTokenOptions
    {
        private int _refreshTokenExpireInMinutes;

        public int RefreshTokenExpireInMinutes
        {
            get => _refreshTokenExpireInMinutes == default ? 10080 : _refreshTokenExpireInMinutes;
            set => _refreshTokenExpireInMinutes = value;
        }
    }
}
