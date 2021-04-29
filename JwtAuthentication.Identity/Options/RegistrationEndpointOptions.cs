
namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RegistrationEndpointOptions
    {
        private string _registrationEndpointRoute;

        public string RegistrationEndpointRoute
        {
            get => _registrationEndpointRoute ?? "/auth/register";
            set => _registrationEndpointRoute = value;
        }
    }
}
