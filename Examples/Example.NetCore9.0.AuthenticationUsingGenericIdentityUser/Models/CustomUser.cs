using Microsoft.AspNetCore.Identity;

namespace Example.NetCore9._0.AuthenticationUsingGenericIdentityUser.Models;

public class CustomUser : IdentityUser<long>
{
    public string Name { get; set; }
}