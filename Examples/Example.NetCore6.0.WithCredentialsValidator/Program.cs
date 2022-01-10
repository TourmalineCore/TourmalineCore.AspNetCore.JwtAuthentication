using Example.NetCore6._0.WithCredentialsValidator;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services
    .AddJwtAuthentication(configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>())
    .AddUserCredentialValidator<UserCredentialsValidator>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app
    .OnLoginExecuting(OnLoginExecuting)
    .OnLoginExecuted(OnLoginExecuted)
    .UseDefaultLoginMiddleware()
    .UseJwtAuthentication();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();

Task OnLoginExecuting(LoginModel data)
{
    Console.WriteLine(data.Login);
    return Task.FromResult(data);
}

Task OnLoginExecuted(LoginModel data)
{
    Console.WriteLine(data.Login);
    return Task.FromResult(data);
}
