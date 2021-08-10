using Microsoft.AspNetCore.Identity;

namespace Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity.Models
{
    public class CustomUser : IdentityUser
    {
        public string Name { get; set; }
    }
}