using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    public class TokenExtractionFromCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CookieAuthOptions _options;

        public TokenExtractionFromCookieMiddleware(RequestDelegate next, CookieAuthOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies[_options.Key];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }

            await _next(context);
        }
    }
}
