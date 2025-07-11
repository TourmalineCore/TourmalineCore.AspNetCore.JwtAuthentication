using Example.Net9._0.RefreshTokenWithConfidenceInterval.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.Net9._0.RefreshTokenWithConfidenceInterval.Data;

public class AppDbContext : TourmalineDbContext<CustomUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}