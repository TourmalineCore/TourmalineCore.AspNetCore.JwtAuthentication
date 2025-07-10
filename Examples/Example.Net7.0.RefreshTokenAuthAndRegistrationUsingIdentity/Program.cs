using Example.Net7._0.RefreshTokenAuthAndRegistrationUsingIdentity.Data;
using Example.Net7._0.RefreshTokenAuthAndRegistrationUsingIdentity.Models;
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
        options.UseInMemoryDatabase("TestDb_Net7")
    );

builder.Services
    .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
    .AddLoginWithRefresh(configuration.GetSection("AuthenticationOptions").Get<RefreshAuthenticationOptions>())
    .AddLogout()
    .AddRegistration();

builder.Services.AddControllers();

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultDbUser<AppDbContext, CustomUser>("Admin", "Admin");

app.UseRouting();

app.UseDefaultLoginMiddleware()
    .UseJwtAuthentication();

app.UseRefreshTokenMiddleware();
app.UseRefreshTokenLogoutMiddleware();

app.UseRegistration(x => new CustomUser
{
    UserName = x.Login,
    NormalizedUserName = x.Login,
}
    );

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();

public partial class Program { }