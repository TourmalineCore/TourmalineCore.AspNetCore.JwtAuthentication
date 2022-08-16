namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    public static class TokenType
    {
        public const string Refresh = "refresh";
        public const string Access = "access";

        public static bool IsAvailableTokenType(string value)
        {
            return value == Refresh || value == Access;
        }
    }
}
