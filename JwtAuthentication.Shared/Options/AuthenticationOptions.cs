namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options
{
    public class AuthenticationOptions
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