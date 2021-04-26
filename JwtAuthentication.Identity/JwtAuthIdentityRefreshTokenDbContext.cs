using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public class JwtAuthIdentityRefreshTokenDbContext<TUser> : JwtAuthIdentityDbContext<TUser> where TUser : IdentityUser
    {
        public JwtAuthIdentityRefreshTokenDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<RefreshToken<TUser>> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken<TUser>>().HasKey(x => x.Id);
            modelBuilder.Entity<RefreshToken<TUser>>().HasIndex(x => x.Value);
            modelBuilder.Entity<RefreshToken<TUser>>()
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<RefreshToken<TUser>>(x => x.UserId);
        }
    }
}
