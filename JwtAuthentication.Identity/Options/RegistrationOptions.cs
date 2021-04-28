
namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class RegistrationOptions
    {
        private string _registrationEndpointRoute;

        public string RegistrationEndpointRoute
        {
            get => _registrationEndpointRoute ?? "/auth/register";
            set => _registrationEndpointRoute = value;
        }
    }
}
