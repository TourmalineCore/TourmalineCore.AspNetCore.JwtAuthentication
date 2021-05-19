using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public class TourmalineDbContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
    {
        public TourmalineDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (TourmalineContextConfiguration.UseRefresh)
            {
                modelBuilder.Model.AddEntityType(typeof(RefreshToken<TUser>));
                modelBuilder.Entity<RefreshToken<TUser>>().HasKey(x => x.Id);
                modelBuilder.Entity<RefreshToken<TUser>>().HasIndex(x => x.Value);
            }
        }
    }
}