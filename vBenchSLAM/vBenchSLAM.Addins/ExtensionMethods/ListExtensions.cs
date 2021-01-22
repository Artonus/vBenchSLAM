using System.Collections.Generic;
using System.Linq;

namespace vBenchSLAM.Addins.ExtensionMethods
{
    public static class ListExtensions
    {
        /// <summary>
        /// Allows to divide source list into list of smaller lists of selected size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source list</param>
        /// <param name="chunkSize">Size to what the list will be devided</param>
        /// <returns></returns>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        /// <summary>
        /// Adds the element to the list if not present
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="item">Element to add</param>
        /// <returns></returns>
        public static void AddIfNotContains<T>(this List<T> source, T item)
        {
            if (source.Contains(item) == false)
                source.Add(item);
        }
    }
}
