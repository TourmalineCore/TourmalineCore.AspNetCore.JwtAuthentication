namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models
{
    public class BasicLoginModel : IBasicLoginModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}
