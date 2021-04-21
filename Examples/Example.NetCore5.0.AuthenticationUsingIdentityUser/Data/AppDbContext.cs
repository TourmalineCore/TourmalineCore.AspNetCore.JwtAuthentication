using Example.NetCore5._0.AuthenticationUsingIdentityUser.Models;
using JwtAuthentication.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Example.NetCore5._0.AuthenticationUsingIdentityUser.Data
{
    public class AppDbContext : JwtAuthIdentityDbContext<CustomUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
