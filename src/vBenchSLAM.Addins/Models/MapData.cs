using System;
using vBenchSLAM.Addins.Abstract;

namespace vBenchSLAM.Addins.Models
{
    public class MapData : ICsvParsable
    {
        /// <summary>
        /// The number of detected Keyframes
        /// </summary>
        public int Keyframes { get; set; }
        /// <summary>
        /// The number of detected key points
        /// </summary>
        public int Keypoints { get; set; }
        /// <summary>
        /// The number of detected landmarks
        /// </summary>
        public int Landmarks { get; set; }

        /// <summary>
        /// <inheritdoc cref="ICsvParsable.GetCsvHeaderRow"/>
        /// </summary>
        /// <returns></returns>
        public string GetCsvHeaderRow()
        {
            return $"{nameof(Keyframes)};{nameof(Keypoints)};{nameof(Landmarks)}";
        }
        /// <summary>
        /// <inheritdoc cref="ICsvParsable.GetCsvHeaderRow"/>
        /// </summary>
        /// <returns></returns>
        public string ParseAsCsvLiteral()
        {
            return $"{Keyframes};{Keypoints};{Landmarks}";
        }
        /// <summary>
        /// Parse the <see cref="MapData"/> from the CSV formatted text
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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
