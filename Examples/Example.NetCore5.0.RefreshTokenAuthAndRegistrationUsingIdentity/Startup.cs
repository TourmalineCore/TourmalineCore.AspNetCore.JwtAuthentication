using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Example.NetCore5._0.AuthenticationWithRefreshToken.Data;
using Example.NetCore5._0.AuthenticationWithRefreshToken.Models;
using Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity.Models;
using Microsoft.EntityFrameworkCore;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.NetCore5._0.RefreshTokenAuthAndRegistrationUsingIdentity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("Database"));

            services
                .AddJwtAuthenticationWithRefreshToken<AppDbContext, CustomUser>()
                .AddLogout<CustomUser>()
                .AddRegistration<CustomUser, CustomRegistrationRequest>();

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
            app.UseRegistration<CustomUser, CustomRegistrationRequest>(x => new CustomUser()
            {
                UserName = x.Login,
                NormalizedUserName = x.Login,
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
