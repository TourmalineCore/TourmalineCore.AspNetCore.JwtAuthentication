using Microsoft.AspNetCore.Identity;

namespace Example.Net9._0.AuthenticationUsingGenericIdentityUser.Models;

public class CustomUser : IdentityUser<long>
{
    public string Name { get; set; }
}