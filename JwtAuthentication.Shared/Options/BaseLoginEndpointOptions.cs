namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options
{
    public class BaseLoginEndpointOptions
    {
        private string _loginEndpointRoute;

        public string LoginEndpointRoute
        {
            get => _loginEndpointRoute ?? "/auth/login";
            set => _loginEndpointRoute = value;
        }
    }
}