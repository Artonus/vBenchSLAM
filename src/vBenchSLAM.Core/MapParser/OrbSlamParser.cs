using System.IO;
using vBenchSLAM.Addins.Models;

namespace vBenchSLAM.Core.MapParser
{
    /// <summary>
    /// Parser for the ORB_SLAM2 map output
    /// </summary>
    public class OrbSlamParser : BaseParser
    {
        /// <summary>
        /// <inheritdoc cref="BaseParser.ParseMap"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public override MapData ParseMap(string file)
        {
            MapData mapData = new MapData();
            mapData.Keyframes = 0;
            
            using (var fr = new StreamReader(file))
            {
                // the line in the file has the following structure:
                // timestamp CameraX CameraY CameraZ RotationX RotationY RotationZ RotationW
                while (fr.ReadLine() is not null)
                {
                    mapData.Keyframes++;
                }
            }
            
            return mapData;
        }
    }
}