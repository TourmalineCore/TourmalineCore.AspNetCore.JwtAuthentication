using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services.AddJwtAuthentication(configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>());
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

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