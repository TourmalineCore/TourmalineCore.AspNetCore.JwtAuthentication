using Example.NetCore6._0.AuthenticationUsingGenericIdentityUser.Data;
using Example.NetCore6._0.AuthenticationUsingGenericIdentityUser.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("Database")
    );

builder.Services
    .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser, long>()
    .AddBaseLogin(configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>())
    .AddLogout()
    .AddRegistration();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultDbUser<AppDbContext, CustomUser, long>("Admin", "Admin");

app.UseRouting();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app
    .UseDefaultLoginMiddleware()
    .UseJwtAuthentication();

app.UseRegistration<CustomUser, long>(x => new CustomUser
        {
            UserName = x.Login,
            NormalizedUserName = x.Login,
        }
    );

app.Run();

public partial class ProgramGeneric { }