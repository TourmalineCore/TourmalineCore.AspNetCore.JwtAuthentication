namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshOptions
    {
        private const int DefaultRefreshConfidenceIntervalInSeconds = 60;
        private int _refreshConfidenceIntervalInSeconds;

        public int RefreshConfidenceIntervalInSeconds
        {
            get => _refreshConfidenceIntervalInSeconds == default ? DefaultRefreshConfidenceIntervalInSeconds : _refreshConfidenceIntervalInSeconds;
            set => _refreshConfidenceIntervalInSeconds = value;
        }
    }
}