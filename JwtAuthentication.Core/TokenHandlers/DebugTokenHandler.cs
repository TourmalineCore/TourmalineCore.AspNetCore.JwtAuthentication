using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.TokenHandlers
{
    internal class DebugTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Schema = "DebugTokenAuthentication";

        public const string HeaderName = "X-DEBUG-TOKEN";

        public DebugTokenHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options,
                    logger,
                    encoder,
                    clock
                )
        {
        }

        /// <summary>
        /// Searches the 'Authorization' header for a 'DebugToken' token. If the 'DebugToken' token is found, it is decoded from base64.
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            try
            {
                string token = Request.Headers[HeaderName];

                if (string.IsNullOrEmpty(token))
                {
                    return Task.FromResult(AuthenticateResult.NoResult());
                }

                var base64EncodedBytes = Convert.FromBase64String(token);
                var decodedToken = Encoding.UTF8.GetString(base64EncodedBytes);

                var claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(decodedToken)
                    .SelectMany(x => x.Value.ValueKind == JsonValueKind.Array
                            ? x.Value.EnumerateArray().Select(e => (x.Key, value: e.GetString()))
                            : new[] { (x.Key, value: x.Value.GetString()) }
                        )
                    .Select(x => new Claim(x.Key, x.value))
                    .ToList();

                var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(claimsIdentity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error while login through DebugToken Scheme");
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }
    }
}