using System;
using System.Collections.Generic;
using System.Reflection;
using vBenchSLAM.Addins.Attributes;

namespace vBenchSLAM.Addins.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the StringValue attributes
            StringValueAttribute[] attributes = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attributes.Length > 0 ? attributes[0].StringValue : null;
        }
        /// <summary>
        /// Gets enum entries as IEnumerable
        /// </summary>
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
