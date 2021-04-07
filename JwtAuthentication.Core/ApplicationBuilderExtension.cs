using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
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
                .UseAuthorization();
        }

        public static IApplicationBuilder UseDefaultLoginMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<LoginMiddleware>();
        }

        public static IApplicationBuilder UseCookieLoginMiddleware(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<CookiesLoginMiddleware>()
                .UseCookiePolicy(new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = SameSiteMode.Strict,
                    HttpOnly = HttpOnlyPolicy.Always,
                    Secure = CookieSecurePolicy.Always,
                })
                .Use(async (context, next) =>
                {
                    var token = context.Request.Cookies[".AspNetCore.Application.Id"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Request.Headers.Add("Authorization", $"Bearer {token}");
                    }

                    await next();
                });
        }
    }
}