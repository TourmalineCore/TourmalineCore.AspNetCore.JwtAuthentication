using Example.Net6._0.AuthenticationUsingIdentityUser.Data;
using Example.Net6._0.AuthenticationUsingIdentityUser.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("Database")
    );

builder.Services
    .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
    .AddBaseLogin(configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>());
builder.Services.AddControllers();

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app
    .UseDefaultLoginMiddleware()
    .UseJwtAuthentication();

app.Run();