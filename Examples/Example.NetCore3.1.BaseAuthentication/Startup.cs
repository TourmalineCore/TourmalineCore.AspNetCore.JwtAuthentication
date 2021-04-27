using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;

namespace Example.NetCore3._1.BaseAuthentication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJwtAuthentication();

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