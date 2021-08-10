namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models
{
    public class LoginModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string ClientFingerPrint { get; set; }
    }
}