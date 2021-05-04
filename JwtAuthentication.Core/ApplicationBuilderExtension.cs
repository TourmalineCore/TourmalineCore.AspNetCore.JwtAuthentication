using Microsoft.AspNetCore.Builder;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core
{
    public static class ApplicationBuilderExtension
    {
        /// <summary>
        /// Adds Authentication and Authorization to the app.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseAuthentication()
                .UseAuthorization();
        }

        /// <summary>
        /// Adds middleware to handle incoming login requests.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="loginEndpointOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultLoginMiddleware(this IApplicationBuilder applicationBuilder, LoginEndpointOptions loginEndpointOptions = null)
        {
            return applicationBuilder
                .UseMiddleware<LoginMiddleware>(loginEndpointOptions ?? new LoginEndpointOptions());
        }

        /// <summary>
        /// Adds middleware to handle incoming login requests using cookies to store auth token.
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCookieLoginMiddleware(this IApplicationBuilder applicationBuilder, CookieAuthOptions options, LoginEndpointOptions loginEndpointOptions = null)
        {
            return applicationBuilder
                .UseMiddleware<LoginWithCookieMiddleware>(options, loginEndpointOptions ?? new LoginEndpointOptions())
                .UseMiddleware<TokenExtractionFromCookieMiddleware>(options);
        }
    }
}