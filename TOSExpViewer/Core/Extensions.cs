using System;
using System.Text.RegularExpressions;

namespace TOSExpViewer.Core
{
    public static class Extensions
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

        /// <summary> Formats a timespan into a shortened output showing hours & minutes, minutes or seconds. </summary>
        public static string ToShortDisplayFormat(this TimeSpan timeSpan)
        {
            if (timeSpan >= TimeSpan.FromDays(1) || timeSpan < TimeSpan.Zero)
            {
                return Constants.INFINITY;
            }

            if (timeSpan >= TimeSpan.FromHours(1))
            {
                return $"{timeSpan.Hours:00}h {timeSpan.Minutes:00}m";
            }

            if (timeSpan >= TimeSpan.FromMinutes(1))
            {
                return $"{timeSpan.Minutes}m";
            }

            return $"{timeSpan.Seconds}s";
        }
    }
}