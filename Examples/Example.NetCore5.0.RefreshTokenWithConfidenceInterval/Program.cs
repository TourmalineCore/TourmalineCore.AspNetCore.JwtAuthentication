using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Example.NetCore5._0.RefreshTokenWithConfidenceInterval
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();

//var configuration = builder.Configuration;
//var environment = builder.Environment;

//var refreshAuthenticationOptions = configuration.GetSection(nameof(AuthenticationOptions)).Get<RefreshAuthenticationOptions>();
//const int refreshConfidenceIntervalInSeconds = 300;

//builder.Services
//    .AddDbContext<AppDbContext>(options =>
//        options.UseInMemoryDatabase("Database")
//    );

//builder.Services
//    .AddJwtAuthenticationWithIdentity<AppDbContext, CustomUser>()
//    .AddLoginWithRefresh(refreshAuthenticationOptions)
//    .AddRefreshConfidenceInterval(refreshConfidenceIntervalInSeconds)
//    .AddLogout()
//    .AddRegistration();

//builder.Services.AddControllers();

//var app = builder.Build();

//if (environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

//app.UseDefaultDbUser<AppDbContext, CustomUser>("Admin", "Admin");

//app.UseRouting();

//app.UseDefaultLoginMiddleware()
//    .UseJwtAuthentication();

//app.UseRefreshTokenMiddleware();
//app.UseRefreshTokenLogoutMiddleware();

//app.UseRegistration(x => new CustomUser
//{
//    UserName = x.Login,
//    NormalizedUserName = x.Login,
//}
//    );

//app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

//app.Run();

//public abstract class ProgramWithConfidenceInterval { }