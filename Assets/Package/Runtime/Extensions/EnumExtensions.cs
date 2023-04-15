using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SparkCore.Runtime.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the enum name.
        /// </summary>
        /// <param name="e">Enum value.</param>
        /// <returns>Enum name.</returns>
        public static string GetDescription (this Enum e)
        {
            return Enum.GetName(e.GetType(), e);
        }
		
        public static T Next<T>(this T v) where T : struct
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }

        public static T Previous<T>(this T v) where T : struct
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).Reverse().SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }
    }
}
