using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshOptions
    {
        private int _refreshConfidenceIntervalInMilliseconds;

        public int RefreshConfidenceIntervalInMilliseconds
        {
            get => _refreshConfidenceIntervalInMilliseconds;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Refresh confidence interval cannot be zero or negative");
                }

                _refreshConfidenceIntervalInMilliseconds = value;
            }
        }
    }
}