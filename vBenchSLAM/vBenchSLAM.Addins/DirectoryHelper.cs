using System;
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

        public static string GetAppDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "vBenchSLAM");
        }

        public static void CreateDirectoryIfNotExists(string documentsPath)
        {
            if (Directory.Exists(documentsPath) == false)
            {
                Directory.CreateDirectory(documentsPath);
            }
        }

        public static string GetMonitorsPath()
        {
            return @$"{GetTempPath()}monitors/";
        }
        public static string GetUserDocumentsFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "vBenchSLAM");
        }
    }
}