using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public class TourmalineDbContext<TUser> : TourmalineDbContext<TUser, string> where TUser : IdentityUser
    {
        public TourmalineDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class TourmalineDbContext<TUser, TKey> : TourmalineDbContext<TUser, IdentityRole<TKey>, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public TourmalineDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class TourmalineDbContext<TUser, TRole, TKey> :
        IdentityDbContext<
            TUser,
            TRole,
            TKey,
            IdentityUserClaim<TKey>,
            IdentityUserRole<TKey>,
            IdentityUserLogin<TKey>,
            IdentityRoleClaim<TKey>,
            IdentityUserToken<TKey>
        >
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private const string DefaultRefreshTokenTableName = "RefreshToken<User>";

        public TourmalineDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (!TourmalineContextConfiguration.UseRefresh)
            {
                return;
            }

            modelBuilder.Model.AddEntityType(typeof(RefreshToken<TUser, TKey>));

            modelBuilder.Entity<RefreshToken<TUser, TKey>>(entity =>
                    {
                        entity.HasKey(x => x.Id);
                        entity.HasIndex(x => x.Value);
                        entity.ToTable(DefaultRefreshTokenTableName);
                    }
                );
        }
    }
}