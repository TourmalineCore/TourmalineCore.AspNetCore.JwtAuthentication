using Example.NetCore8._0.AuthenticationUsingGenericIdentityUser.Data;
using Example.NetCore8._0.AuthenticationUsingGenericIdentityUser.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var configuration = builder.Configuration;
var environment = builder.Environment;

var refreshAuthenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<RefreshAuthenticationOptions>();

var accessTokenExpireInMinutes = refreshAuthenticationOptions.AccessTokenExpireInMinutes;

builder.Services
    .AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("Database")
        );

builder.Services
    .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser, long>()
    .AddLoginWithRefresh(configuration.GetSection("AuthenticationOptions").Get<RefreshAuthenticationOptions>())
    .AddLogout()
    .AddRegistration();


builder.Services.AddControllers();

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultDbUser<AppDbContext, CustomUser, long>("Admin", "Admin");

app.UseRouting();

app.UseDefaultLoginMiddleware()
    .UseJwtAuthentication();

app.UseRefreshTokenMiddleware();
app.UseRefreshTokenLogoutMiddleware();

app.UseRegistration<CustomUser, long>(x => new CustomUser
{
    UserName = x.Login,
    NormalizedUserName = x.Login,
}
    );

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();

public partial class ProgramGeneric { }