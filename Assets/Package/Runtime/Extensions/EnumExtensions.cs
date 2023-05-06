using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SparkCore.Runtime.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of the given enum value.
        /// </summary>
        /// <param name="e">The enum value to get the description for.</param>
        /// <returns>The description of the enum value.</returns>
        public static string GetDescription(this Enum e)
        {
            return Enum.GetName(e.GetType(), e);
        }

        /// <summary>
        /// Gets the next value of the given enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="v">The current value of the enum.</param>
        /// <returns>The next value of the enum.</returns>
        public static T Next<T>(this T v) where T : struct
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).SkipWhile(e => !v.Equals(e))
                .Skip(1).First();
        }

        /// <summary>
        /// Gets the previous value of the given enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="v">The current value of the enum.</param>
        /// <returns>The previous value of the enum.</returns>
        public static T Previous<T>(this T v) where T : struct
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).Reverse()
                .SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }

        /// <summary>
        /// Gets the first value of the given enum type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumeration">The enum type to get the first value for.</param>
        /// <returns>The first value of the enum type.</returns>
        public static T GetFirstValue<T>(this T enumeration) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the last value of the given enum type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumeration">The enum type to get the last value for.</param>
        /// <returns>The last value of the enum type.</returns>
        public static T GetLastValue<T>(this T enumeration) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().LastOrDefault();
        }

        /// <summary>
        /// Gets the default value of the given enum type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumeration">The enum type to get the default value for.</param>
        /// <returns>The default value of the enum type.</returns>
        public static T GetDefaultValue<T>(this T enumeration) where T : Enum
        {
            return default(T);
        }

        /// <summary>
        /// Gets a random value from the given enum type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumeration">The enum type to get a random value from.</param>
        /// <returns>A random value from the enum type.</returns>
        public static T GetRandomValue<T>(this T enumeration) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(new Random().Next(values.Length));
        }

        /// <summary>
        /// Parses the given string into an enum value of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="enumeration">The enum type to parse the value for
        /// <param name="value">The string value to parse.</param>
        /// <returns>An enum value of the specified type.</returns>
        public static T Parse<T>(this T enumeration, string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Determines if the given enum value is the first value in the enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <returns>True if the enum value is the first value in the enum, false otherwise.</returns>
        public static bool IsFirst<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            var minValue = Enum.GetValues(enumValue.GetType()).OfType<TEnum>().Min();
            return enumValue.Equals(minValue);
        }

        /// <summary>
        /// Determines if the given enum value is the last value in the enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <returns>True if the enum value is the last value in the enum, false otherwise.</returns>
        public static bool IsLast<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            var maxValue = Enum.GetValues(enumValue.GetType()).OfType<TEnum>().Max();
            return enumValue.Equals(maxValue);
        }
    }
}
