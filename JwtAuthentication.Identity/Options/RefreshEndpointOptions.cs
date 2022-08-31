using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RefreshEndpointOptions : IRefreshEndpointOptions
    {
        private string _refreshEndpointRoute;

        public string RefreshEndpointRoute
        {
            get => _refreshEndpointRoute ?? "/auth/refresh";
            set => _refreshEndpointRoute = value;
        }
    }
}