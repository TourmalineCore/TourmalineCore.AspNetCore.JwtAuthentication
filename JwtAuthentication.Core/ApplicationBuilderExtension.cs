using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core
{
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Adds middleware to handle incoming login requests.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseAuthentication()
                .UseAuthorization();
        }

        public static IApplicationBuilder UseDefaultLoginMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<LoginMiddleware>();
        }

        public static IApplicationBuilder UseCookieLoginMiddleware(this IApplicationBuilder applicationBuilder, CookieAuthOptions options)
        {
            return applicationBuilder
                .UseMiddleware<CookieLoginMiddleware>(options)
                .UseMiddleware<CookieTokenMiddleware>(options);
        }
    }
}