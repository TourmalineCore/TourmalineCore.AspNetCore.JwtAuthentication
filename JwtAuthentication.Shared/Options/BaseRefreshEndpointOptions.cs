namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options
{
    public class BaseRefreshEndpointOptions
    {
        private string _refreshEndpointRoute;

        public string RefreshEndpointRoute
        {
            get => _refreshEndpointRoute ?? "/auth/refresh";
            set => _refreshEndpointRoute = value;
        }
    }
}