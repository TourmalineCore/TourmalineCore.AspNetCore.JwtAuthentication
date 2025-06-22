using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services
    .AddJwtAuthentication(configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>());

var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCookieLoginMiddleware(new CookieAuthOptions
        {
            Key = "ExampleCookieName",
        }
    );

app.UseJwtAuthentication();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class ProgramCookies { }
