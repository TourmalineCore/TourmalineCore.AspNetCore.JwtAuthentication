using Example.Net5._0.RefreshTokenAuthAndRegistrationUsingIdentity.Data;
using Example.Net5._0.RefreshTokenAuthAndRegistrationUsingIdentity.Models;
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

namespace Example.Net5._0.RefreshTokenAuthAndRegistrationUsingIdentity
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
            var opt = _configuration.GetSection(nameof(AuthenticationOptions)).Get<RefreshAuthenticationOptions>();

            var a = opt.AccessTokenExpireInMinutes;

            services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_Net5")
                );

            services
                .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
                .AddLoginWithRefresh(_configuration.GetSection("AuthenticationOptions").Get<RefreshAuthenticationOptions>())
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