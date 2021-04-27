namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Options
{
    public class AuthenticationOptions
    {
        private string _signingKey;

        private int _accessTokenExpireInMinutes;

        public string SigningKey
        {
            get => _signingKey ?? "jwtKeyjwtKeyjwtKeyjwtKeyjwtKey";
            set => _signingKey = value;
        }

        public string Issuer { get; set; }

        public int AccessTokenExpireInMinutes
        {
            get => _accessTokenExpireInMinutes == default ? 10080 : _accessTokenExpireInMinutes;
            set => _accessTokenExpireInMinutes = value;
        }

        public bool IsDebugTokenEnabled { get; set; }
    }
}