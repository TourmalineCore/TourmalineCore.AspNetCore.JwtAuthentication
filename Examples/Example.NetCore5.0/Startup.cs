using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

namespace Example.NetCore5._0
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJwtAuthentication()
                .AddCustomAuthorization();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseJwtAuthentication()
                .WithRegistration()
                .WithRoleManagement();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}