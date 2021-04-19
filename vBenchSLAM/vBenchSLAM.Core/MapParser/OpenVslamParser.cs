using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using Serilog;
using vBenchSLAM.Core.MapParser.Models;

namespace vBenchSLAM.Core.MapParser
{
    public class OpenVslamParser : BaseParser
    {
        public override MapData ParseMap(string file)
        {
            return GetMapDataFromMessagePack(file);
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
    }
}