using Microsoft.AspNetCore.Identity;

namespace Example.NetCore5._0.AuthenticationUsingIdentityUser.Models
{
    public class CustomUser : IdentityUser
    {
        public string Name { get; set; }
    }
}