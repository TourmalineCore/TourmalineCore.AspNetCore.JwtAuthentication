namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshOptions
    {
        private const int SecondsPerMinute = 60;
        private int _refreshConfidenceIntervalInSeconds;

        public int RefreshConfidenceIntervalInSeconds
        {
            get => _refreshConfidenceIntervalInSeconds == default ? SecondsPerMinute : _refreshConfidenceIntervalInSeconds;
            set => _refreshConfidenceIntervalInSeconds = value;
        }
    }
}