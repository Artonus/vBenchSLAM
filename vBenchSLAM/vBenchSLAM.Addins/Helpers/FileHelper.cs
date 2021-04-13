using System.IO;

namespace vBenchSLAM.Addins
{
    public class FileHelper
    {
        /// <summary>
        /// Safely copies the file. If the file exists at the destination, overrides it 
        /// </summary>
        public static void SafeCopy(string source, string destination)
        {
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }
            File.Copy(source, destination);
        }
    }
}