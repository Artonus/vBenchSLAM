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
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "vBenchSLAM");
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

        /// <summary>
        /// Copies the content and the structure of the whole subdirectory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyFullDir(DirectoryInfo source, DirectoryInfo target)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo?redirectedfrom=MSDN&view=net-5.0
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyFullDir(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}