namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public static class TourmalineContextConfiguration
    {
        public static bool UseRefresh { get; set; } = false;

        public static bool UseRefreshConfidenceInterval { get; set; } = false;
    }
}