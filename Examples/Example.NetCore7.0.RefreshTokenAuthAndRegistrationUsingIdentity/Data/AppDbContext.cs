using Example.NetCore7._0.RefreshTokenAuthAndRegistrationUsingIdentity.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.NetCore7._0.RefreshTokenAuthAndRegistrationUsingIdentity.Data;

public class AppDbContext : TourmalineDbContext<CustomUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
