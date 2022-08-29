using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace Example.NetCore3._1.BaseAuthentication
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
            services.AddJwtAuthentication(_configuration.GetSection(nameof(AuthenticationOptions)).Get<AuthenticationOptions>());

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //app
            //    .OnLoginExecuting(OnLoginExecuting)
            //    .OnLoginExecuted(OnLoginExecuted)
            //    .UseDefaultLoginMiddleware()
            //    .UseJwtAuthentication();

            //app
            //    .UseDefaultLoginMiddleware()
            //    .OnLoginExecuting(OnLoginExecuting)
            //    .OnLoginExecuted(OnLoginExecuted)
            //    .UseJwtAuthentication();

            //app
            //    .OnLoginExecuting(OnLoginExecuting)
            //    .UseDefaultLoginMiddleware()
            //    .OnLoginExecuted(OnLoginExecuted)
            //    .UseJwtAuthentication();

            //app
            //    .OnLoginExecuting(OnLoginExecuting)
            //    .OnLoginExecuted(OnLoginExecuted)
            //    .UseDefaultLoginMiddleware()
            //    .UseJwtAuthentication();

            //app
            //    .OnLoginExecuted(OnLoginExecuted)
            //    .UseDefaultLoginMiddleware()
            //    .UseJwtAuthentication();

            //app
            //    .OnLoginExecuting(OnLoginExecuting)
            //    .OnLoginExecuted(OnLoginExecuted);

            //app
            //    .UseDefaultLoginMiddleware()
            //    .UseJwtAuthentication();

            app.UseCookieLoginMiddleware(new CookieAuthOptions
            {
                Key = "ExampleCookieName",
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private Task OnLoginExecuting(LoginModel data)
        {
            Console.WriteLine($"OnLoginExecuting: {data.Login}");
            return Task.CompletedTask;
        }

        private Task OnLoginExecuted(LoginModel data)
        {
            Console.WriteLine($"OnLoginExecuted: {data.Login}");
            return Task.CompletedTask;
        }
    }
}