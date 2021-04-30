using Example.NetCore5._0.AuthenticationUsingIdentityUser.Data;
using Example.NetCore5._0.AuthenticationUsingIdentityUser.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity;

namespace Example.NetCore5._0.AuthenticationUsingIdentityUser
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
                .AddBaseLogin();

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

            app
                .UseDefaultLoginMiddleware()
                .UseJwtAuthentication();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}