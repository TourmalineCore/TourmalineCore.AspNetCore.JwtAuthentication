using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Middlewares.Login.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models
{
    public class LoginModel : IBasicLoginModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ClientFingerPrint { get; set; }

        public static LoginModel MapFrom(BasicLoginModel basicLoginModel)
        {
            return new LoginModel
            {
                Login = basicLoginModel.Login,
                Password = basicLoginModel.Password,
                ClientFingerPrint = basicLoginModel.ClientFingerPrint
            };
        }
    }
}