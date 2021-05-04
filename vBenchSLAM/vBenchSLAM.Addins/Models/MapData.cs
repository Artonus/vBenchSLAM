using System;
using vBenchSLAM.Addins.Abstract;

namespace vBenchSLAM.Addins.Models
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

        public static MapData FromCsvLiteral(string line)
        {
            var spitted = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var model = new MapData();
            model.Keyframes = int.Parse(spitted[0]);
            model.Keypoints = int.Parse(spitted[1]);
            model.Landmarks = int.Parse(spitted[2]);
            return model;
        }

    }
}
