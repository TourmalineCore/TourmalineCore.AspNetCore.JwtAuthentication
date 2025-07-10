using Microsoft.AspNetCore.Identity;

namespace Example.Net7._0.AuthenticationUsingGenericIdentityUser.Models;

public class CustomUser : IdentityUser<long>
{
    public string Name { get; set; }
}