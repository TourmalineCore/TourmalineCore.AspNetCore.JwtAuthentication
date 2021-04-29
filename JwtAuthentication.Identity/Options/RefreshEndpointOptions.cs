using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshEndpointOptions
    {
        private string _refreshEndpointRoute;

        public string RefreshEndpointRoute
        {
            get => _refreshEndpointRoute ?? "/auth/refresh";
            set => _refreshEndpointRoute = value;
        }
    }
}