using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.MapParser.Models
{
    public class MapData : ICsvParsable
    {
        public int Keyframes { get; set; }
        public int Keypoints { get; set; }
        public int Landmarks { get; set; }


        public string GetCsvHeaderRow()
        {
            return $"{nameof(Keyframes)};{nameof(Keypoints)};{nameof(Landmarks)}";
        }

        public string ParseAsCsvLiteral()
        {
            return $"{Keyframes};{Keypoints};{Landmarks}";
        }
    }
}
