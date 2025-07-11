using Example.Net7._0.AuthenticationWithRefreshToken.Data;
using Example.Net7._0.AuthenticationWithRefreshToken.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Logout.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Middleware.Refresh.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

var builder = WebApplication.CreateBuilder(args);



var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("Database")
    );

builder.Services
    .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
    .AddLoginWithRefresh(configuration.GetSection("AuthenticationOptions").Get<RefreshAuthenticationOptions>())
    .AddLogout();

builder.Services.AddControllers();

var app = builder.Build();



if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultDbUser<AppDbContext, CustomUser>("Admin", "Admin");

app.UseRouting();

app.UseDefaultLoginMiddleware();

app
    .OnLogoutExecuted(OnLogoutExecuted)
    .OnLogoutExecuting(OnLogoutExecuting)
    .UseLogoutMiddleware();

app
    .OnRefreshExecuting(OnRefreshExecuting)
    .UseRefreshMiddleware();

app.UseJwtAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.MapControllers();

app.Run();

Task OnLogoutExecuting(LogoutModel data)
{
    return Task.CompletedTask;
}

Task OnLogoutExecuted(LogoutModel data)
{
    return Task.CompletedTask;
}

Task OnRefreshExecuting(RefreshModel data)
{
    return Task.CompletedTask;
}