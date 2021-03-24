using System;

namespace vBenchSLAM.Addins.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the time formatted as yyyyMMddHHmmss
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>String formatted as yyyyMMddHHmmss</returns>
        public static string FormatAsFileNameCode(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss");
        }
    }
}