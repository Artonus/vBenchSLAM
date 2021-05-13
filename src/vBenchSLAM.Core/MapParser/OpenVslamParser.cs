using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using Serilog;
using vBenchSLAM.Addins.Models;

namespace vBenchSLAM.Core.MapParser
{
    /// <summary>
    /// Parser for the OpenVSLAM map output
    /// </summary>
    public class OpenVslamParser : BaseParser
    {
        /// <summary>
        /// <inheritdoc cref="BaseParser.ParseMap"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public override MapData ParseMap(string file)
        {
            return GetMapDataFromMessagePack(file);
        }
        /// <summary>
        /// Parses the MessagePack file to retrieve the <see cref="MapData"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public MapData GetMapDataFromMessagePack(string file)
        {
            var map = new MapData();
            try
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    dynamic data = MessagePackSerializer.Deserialize<dynamic>(fs);                    
                
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
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to serialize");
            }

            return map;
        }
    }
}