using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using Serilog;
using vBenchSLAM.Core.MapParser.Models;

namespace vBenchSLAM.Core.MapParser
{
    public class BaseParser
    {
        public BaseParser()
        {

        }

        public MapData GetMapDataFromMessagePack(string file)
        {
            var map = new MapData();
            try
            {
                byte[] bytes = File.ReadAllBytes(file);

                dynamic data = MessagePackSerializer.Deserialize<dynamic>(bytes);
                
                
                if (data["keyframes"] is ICollection keyframesCollection)
                {
                    map.Keyframes = keyframesCollection.Count;
                    int count = 0;
                    foreach (KeyValuePair<object, object> frame in keyframesCollection)
                    {
                        var localCnt = (frame.Value as dynamic)?["n_keypts"];
                        count += localCnt;
                    }

                    map.Keypoints = count;
                }

                if (data["landmarks"] is ICollection landmarkCollection)
                {
                    map.Landmarks = landmarkCollection.Count;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to serialize");
            }

            return map;
        }

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
