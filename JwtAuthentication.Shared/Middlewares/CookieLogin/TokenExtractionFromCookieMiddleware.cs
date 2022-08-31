using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.CookieLogin
{
    public class TokenExtractionFromCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICookieAuthOptions _options;

        public TokenExtractionFromCookieMiddleware(RequestDelegate next, ICookieAuthOptions options)
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