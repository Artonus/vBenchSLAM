using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MessagePack;

namespace vBenchSLAM.Core.MapParser
{
    public class MapParser
    {
        public MapParser()
        {
            
        }

        public void ConvertBinaryToString(string path, string outputPath)
        {
            bool retVal = true;
            try
            {
                var bytes = File.ReadAllBytes(path);

                var json = MessagePackSerializer.ConvertToJson(bytes);

                using (var fw = new StreamWriter(outputPath))
                {
                    fw.WriteLine(json);
                }


                Console.WriteLine("Converted to binary");
            }
            catch (Exception ex)
            {
                retVal = false;
                Console.WriteLine();
            }
        }

        public void LoadObjectFromParsedMap(string path, string outputPath)
        {
            bool retVal = true;
            try
            {
                var bytes = File.ReadAllBytes(path);

                var deserialized = MessagePackSerializer.Deserialize<object>(bytes);

                using (var fw = new StreamWriter(outputPath))
                {
                    fw.WriteLine(deserialized);
                }


                Console.WriteLine("Converted to binary");
            }
            catch (Exception ex)
            {
                retVal = false;
                Console.WriteLine();
            }
        }
    }
}
