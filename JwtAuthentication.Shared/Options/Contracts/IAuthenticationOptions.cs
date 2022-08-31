namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts
{
    public interface IAuthenticationOptions
    {
        public string PrivateSigningKey { get; set; }

        public string PublicSigningKey { get; set; }        

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int AccessTokenExpireInMinutes { get; set; }

        public bool IsDebugTokenEnabled { get; set; }
    }
}