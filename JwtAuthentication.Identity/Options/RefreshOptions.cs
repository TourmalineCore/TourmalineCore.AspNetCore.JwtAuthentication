namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshOptions
    {
        private const int SecondsPerMinute = 60;

        private bool _useRefreshConfidenceInterval;
        private int _refreshConfidenceIntervalInSeconds;

        public bool UseRefreshConfidenceInterval
        {
            get => _useRefreshConfidenceInterval;
            set => _useRefreshConfidenceInterval = value;
        }

        public int RefreshConfidenceIntervalInSeconds
        {
            get => _refreshConfidenceIntervalInSeconds == default ? SecondsPerMinute : _refreshConfidenceIntervalInSeconds;
            set => _refreshConfidenceIntervalInSeconds = value;
        }
    }
}