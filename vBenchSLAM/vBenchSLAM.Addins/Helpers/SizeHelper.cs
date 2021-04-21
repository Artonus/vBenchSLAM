using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace vBenchSLAM.Addins
{
    public enum Size
    {
        bytes,
        KB,
        MB,
        GB,
        TB,
        PB,
        EB,
        ZB,
        YB
        
    }
    public static class SizeHelper
    {
        public static string GetSizeSuffix(ulong value, int decimalPlaces = 1)
        {
            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, ((Size)i).ToString());
        }

        public static decimal SizeValue(ulong value, Size size)
        {
            int idx = (int) size;
            
            decimal dValue = value;
            
            for (int i = 0; i < idx; i++)
            {
                dValue /= 1024;
            }

            return dValue;
        }
    }
}