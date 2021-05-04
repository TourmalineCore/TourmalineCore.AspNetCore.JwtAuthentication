namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Options
{
    public class LoginEndpointOptions
    {
        private string _loginEndpointRoute;

        public string LoginEndpointRoute
        {
            get => _loginEndpointRoute ?? "/auth/login";
            set => _loginEndpointRoute = value;
        }
    }
}