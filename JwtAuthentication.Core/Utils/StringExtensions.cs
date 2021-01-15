using System.Text;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Utils
{
    public static class StringExtensions
    {
        public static byte[] ToEncodedByteArray(this string source)
        {
            return Encoding.UTF8.GetBytes(source);
        }
    }
}