namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options
{
    public class RefreshTokenOptions
    {
        private int _refreshTokenExpireInMinutes;

        public int RefreshTokenExpireInMinutes
        {
            get => _refreshTokenExpireInMinutes == default ? 10080 : _refreshTokenExpireInMinutes;
            set => _refreshTokenExpireInMinutes = value;
        }
    }
}
