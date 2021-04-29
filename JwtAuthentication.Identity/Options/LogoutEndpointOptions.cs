namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class LogoutEndpointOptions
    {
        private string _logoutEndpointRoute;

        public string LogoutEndpointRoute
        {
            get => _logoutEndpointRoute ?? "/auth/logout";
            set => _logoutEndpointRoute = value;
        }
    }
}