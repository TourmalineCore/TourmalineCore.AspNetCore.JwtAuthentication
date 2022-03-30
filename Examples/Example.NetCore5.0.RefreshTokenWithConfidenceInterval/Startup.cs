using Example.NetCore5._0.RefreshTokenWithConfidenceInterval.Data;
using Example.NetCore5._0.RefreshTokenWithConfidenceInterval.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options;

namespace Example.NetCore5._0.RefreshTokenWithConfidenceInterval
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
            const int refreshConfidenceIntervalInSeconds = 300;

            services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("Database")
                );

            services
                .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
                .AddLoginWithRefresh(_configuration.GetSection(nameof(AuthenticationOptions)).Get<RefreshAuthenticationOptions>())
                .AddRefreshConfidenceInterval(refreshConfidenceIntervalInSeconds)
                .AddLogout()
                .AddRegistration();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
        }
    }
}