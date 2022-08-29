using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options
{
    public class BaseCookieAuthOptions : ICookieAuthOptions
    {
        public string Key { get; set; }
    }
}