using System.IO;

namespace vBenchSLAM.Addins
{
    public static class DirectoryHelper
    {
        public static string GetTempPath()
        {
            return @$"{Path.GetTempPath()}vBenchSLAM/";
        }

        public static string GetDataFolderPath()
        {
            string dir = $"{GetTempPath()}data/";
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public static void ClearDataFolder()
        {
            string dir = GetDataFolderPath();
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(dir, true);
        }
    }
}