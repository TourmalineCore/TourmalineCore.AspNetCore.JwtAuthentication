using Example.NetCore7._0.AuthenticationWithRefreshToken.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.NetCore7._0.AuthenticationWithRefreshToken.Data
{
    public class AppDbContext : TourmalineDbContext<CustomUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
