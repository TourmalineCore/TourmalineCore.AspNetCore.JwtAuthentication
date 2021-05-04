using Example.NetCore5._0.AuthenticationWithRefreshToken.Models;
using Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("Database")
                );

            services
                .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
                .AddLoginWithRefresh()
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

            app.UseJwtAuthentication();
            app.UseDefaultLoginMiddleware();
            app.UseRefreshTokenMiddleware();
            app.UseRefreshTokenLogoutMiddleware();

            app.UseRegistration<CustomUser>(x => new CustomUser
                    {
                        UserName = x.Login,
                        NormalizedUserName = x.Login,
                    }
                );

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}