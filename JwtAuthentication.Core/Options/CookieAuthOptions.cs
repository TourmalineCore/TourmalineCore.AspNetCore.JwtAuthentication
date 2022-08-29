using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Options.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Options
{
    public class CookieAuthOptions : ICookieAuthOptions
    {
        public string Key { get; set; }
    }
}