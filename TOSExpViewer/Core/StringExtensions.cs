using System;
using System.Text.RegularExpressions;

namespace TOSExpViewer.Core
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns spaced output for a Pascal cased string value
        /// <example>"SomePascalCaseString" returns as "Some Pascal Case String"</example>
        /// </summary>
        /// <param name="pascalCaseValue">A string in Pascal case format</param>
        /// <returns>Spaced Pascal case string</returns>
        public static string ToFriendlyString(this string pascalCaseValue)
        {
            return Regex.Replace(pascalCaseValue, "(?!^)([A-Z])", " $1");
        }

        public static string ReverseFriendlyString(this string friendlyStringValue)
        {
            if (string.IsNullOrWhiteSpace(friendlyStringValue))
            {
                return friendlyStringValue;
            }

            return friendlyStringValue.Replace(" ", string.Empty);
        }

        public static bool IsEqualTo(this string original, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return String.Compare(original, value, stringComparison) == 0;
        }
    }
}