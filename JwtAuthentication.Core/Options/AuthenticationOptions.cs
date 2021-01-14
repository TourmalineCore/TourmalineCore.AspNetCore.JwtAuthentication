namespace JwtAuthentication.Core.Options
{
    public class AuthenticationOptions
    {
        private string _signingKey;
        public string SigningKey
        {
            get => _signingKey ?? "jwtKeyjwtKeyjwtKeyjwtKeyjwtKey";
            set => _signingKey = value;
        }


        private string _issuer;
        public string Issuer
        {
            get => _issuer;
            set => _issuer = value;
        }

        private int _accessTokenExpireInMinutes;
        public int AccessTokenExpireInMinutes
        {
            get => _accessTokenExpireInMinutes == default ? 10080 : _accessTokenExpireInMinutes;
            set => _accessTokenExpireInMinutes = value;
        }

        public bool IsDebugTokenEnabled { get; set; } 
    }
}