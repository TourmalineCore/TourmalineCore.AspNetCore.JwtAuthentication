using Example.Net9._0.AuthenticationUsingGenericIdentityUser.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.Net9._0.AuthenticationUsingGenericIdentityUser.Data;

public class AppDbContext : TourmalineDbContext<CustomUser, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}