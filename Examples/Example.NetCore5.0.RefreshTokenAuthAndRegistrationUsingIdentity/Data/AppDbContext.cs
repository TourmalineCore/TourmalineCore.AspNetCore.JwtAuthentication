using Example.NetCore5._0.AuthenticationWithRefreshToken.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.NetCore5._0.AuthenticationWithRefreshToken.Data
{
    public class AppDbContext : JwtAuthIdentityRefreshTokenDbContext<CustomUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}