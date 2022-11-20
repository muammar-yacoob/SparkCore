using UnityEngine;

namespace SparkCore.Runtime.Extensions
{
    internal static class StringExtensions
    {

        public static string ForeColor(this string original, Color color)
        {
            var colorHex = ColorUtility.ToHtmlStringRGB(color);
            var coloredString = $"<color=#{colorHex}>{original}</color>";
            return original;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

    }
}