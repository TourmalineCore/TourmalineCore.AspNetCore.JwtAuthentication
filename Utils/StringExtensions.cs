using System;
using System.Globalization;
using System.Text;

namespace Utils
{
    public static class StringExtensions
    {
        public static byte[] ToEncodedByteArray(this string source)
        {
            return Config.DefaultEncoding.GetBytes(source);
        }

        public static bool IsNameEqual(this string firstName, string secondName)
        {
            return string.Equals(firstName.NormalizeString(), secondName.NormalizeString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static double GetParsedDoubleInvariant(this string value)
        {
            if (TryParseDoubleWithComma(value, out var result) == false)
            {
                TryParseDoubleWithDot(value, out result);
            }

            return result;
        }

        public static double? GetParsedDoubleOrNullInvariant(this string value)
        {
            if (TryParseDoubleWithComma(value, out var result))
            {
                return result;
            }

            if (TryParseDoubleWithDot(value, out result))
            {
                return result;
            }

            return null;
        }

        public static string NormalizeLowerCaseString(this string str)
        {
            return str.NormalizeString().ToLowerInvariant();
        }

        public static string RemoveControlSymbols(this string value)
        {
            var sb = new StringBuilder(value.Length);

            foreach (var currentSymbol in value)
            {
                if (!char.IsControl(currentSymbol))
                {
                    sb.Append(currentSymbol);
                }
            }

            return sb.ToString();
        }

        private static string NormalizeString(this string str)
        {
            var trimmedStr = str?.Trim();

            return trimmedStr.IsNullOrEmptyOrWhitespace()
                ? string.Empty
                : trimmedStr;
        }

        private static bool IsNullOrEmptyOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        private static bool TryParseDoubleWithComma(string value, out double result)
        {
            return double.TryParse(
                    value,
                    NumberStyles.Any,
                    new CultureInfo("ru"),
                    out result
                );
        }

        private static bool TryParseDoubleWithDot(string value, out double result)
        {
            return double.TryParse(
                    value,
                    NumberStyles.Any,
                    new CultureInfo("en"),
                    out result
                );
        }
    }
}