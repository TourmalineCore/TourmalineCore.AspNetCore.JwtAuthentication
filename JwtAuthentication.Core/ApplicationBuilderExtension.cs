using JwtAuthentication.Core.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace JwtAuthentication.Core
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<LoginMiddleware>();
        }
    }
}