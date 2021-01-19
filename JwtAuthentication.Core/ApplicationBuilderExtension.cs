using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;

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
                .UseAuthorization()
                .UseMiddleware<LoginMiddleware>();
        }

        public static IApplicationBuilder WithRegistration(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<RegistrationMiddleware>();
        }

        public static IApplicationBuilder WithRoleManagement(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<RoleCreationMiddleware>();
        }
    }
}