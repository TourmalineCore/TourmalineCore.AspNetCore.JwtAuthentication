using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace Example.NetCore3._1.PermissionBasedAuthentication
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddJwtAuthentication(_configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>())
                .WithUserClaimsProvider<CustomUserClaimsProvider>(CustomUserClaimsProvider.ExampleClaimType);

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app
                .UseDefaultLoginMiddleware()
                .UseJwtAuthentication();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
