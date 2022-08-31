using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Options
{
    public class AuthenticationOptions : IAuthenticationOptions
    {
        private int _accessTokenExpireInMinutes;

        public string PublicSigningKey { get; set; }

        public string PrivateSigningKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public virtual int AccessTokenExpireInMinutes
        {
            get => _accessTokenExpireInMinutes == default ? 10080 : _accessTokenExpireInMinutes;
            set => _accessTokenExpireInMinutes = value;
        }

        public bool IsDebugTokenEnabled { get; set; }
    }
}