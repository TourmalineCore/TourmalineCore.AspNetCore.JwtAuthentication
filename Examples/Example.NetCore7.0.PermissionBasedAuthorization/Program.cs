using Example.NetCore7._0.PermissionBasedAuthorization;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services
    .AddJwtAuthentication(configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>())
    .WithUserClaimsProvider<UserClaimsProvider>(UserClaimsProvider.ExampleClaimType);

builder.Services.AddControllers();

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

app
    .UseDefaultLoginMiddleware()
    .UseJwtAuthentication();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();

