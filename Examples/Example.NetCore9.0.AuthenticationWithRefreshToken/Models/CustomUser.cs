using Microsoft.AspNetCore.Identity;

namespace Example.NetCore9._0.AuthenticationWithRefreshToken.Models
{
    public class CustomUser : IdentityUser
    {
        public string Name { get; set; }
    }
}

