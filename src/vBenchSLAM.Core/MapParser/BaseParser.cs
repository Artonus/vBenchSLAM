using System;
using System.Collections;
using vBenchSLAM.Addins.Models;

namespace vBenchSLAM.Core.MapParser
{
    /// <summary>
    /// Base class for the map parsers
    /// </summary>
    public abstract class BaseParser
    {
        public BaseParser()
        {

        }
        /// <summary>
        /// Parse the map data created by the algorithm
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public abstract MapData ParseMap(string file);

        [Obsolete]
        private static void Iterate(object obj)
        {
            var idic = (IDictionary)obj;
            foreach (var key in idic.Keys)
            {
                if (idic[key] is IDictionary)
                {
                    Iterate(idic[key]);
                }
                else if (idic[key].GetType().IsArray)
                {
                    string arryText = CombineArray(idic[key]);
                    Console.WriteLine($"Key: {key}, Value: {arryText}");
                }
                else
                    Console.WriteLine($"Key: {key}, Value: {idic[key]}");
            }
        }
        [Obsolete]
        private static string CombineArray(object obj)
        {
            IEnumerable arr = obj as IEnumerable;
            if (arr is null)
            {
                return $"[]";
            }

            string retVal = "[";
            foreach (var entry in arr)
            {
                if (entry is IEnumerable)
                {
                    retVal += CombineArray(entry) + ",";
                }
                else
                    retVal += entry + ",";
            }

            retVal += "]";
            return retVal;
        }
    }
}
