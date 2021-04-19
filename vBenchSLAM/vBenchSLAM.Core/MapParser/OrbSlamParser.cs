using System.IO;
using vBenchSLAM.Core.MapParser.Models;

namespace vBenchSLAM.Core.MapParser
{
    public class OrbSlamParser : BaseParser
    {
        public override MapData ParseMap(string file)
        {
            MapData mapData = new MapData();
            mapData.Keyframes = 0;
            
            using (var fr = new StreamReader(file))
            {
                // the line in the file has the following structure:
                // timestamp CameraX CameraY CameraZ RotationX RotationY RotationZ RotationW
                string line;
                while (fr.ReadLine() is not null)
                {
                    mapData.Keyframes++;
                }
            }

            return mapData;
        }
    }
}