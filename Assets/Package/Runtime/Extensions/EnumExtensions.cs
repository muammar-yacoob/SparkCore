using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SparkCore.Runtime.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Extension method for getting "friendly" enum names:
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }

            return name;
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