using System;
using System.IO;

namespace vBenchSLAM.Addins
{
    /// <summary>
    /// Common directory operations
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// Returns the vBenchSLAM temporary catalog
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            return @$"{Path.GetTempPath()}vBenchSLAM/";
        }
        /// <summary>
        /// Returns the vBenchSLAM temporary catalog that holds the data
        /// </summary>
        /// <returns></returns>
        public static string GetDataFolderPath()
        {
            string dir = $"{GetTempPath()}data/";
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }
        /// <summary>
        /// Clears the content of the temporary folder of the application
        /// </summary>
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
        /// <summary>
        /// Get the vBenchSLAM application data folder
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "vBenchSLAM");
        }
        /// <summary>
        /// Creates directory if not exist
        /// </summary>
        /// <param name="documentsPath"></param>
        public static void CreateDirectoryIfNotExists(string documentsPath)
        {
            if (Directory.Exists(documentsPath) == false)
            {
                Directory.CreateDirectory(documentsPath);
            }
        }
        /// <summary>
        /// Get temporary folder for the resource monitor
        /// </summary>
        /// <returns></returns>
        public static string GetResourceMonitorsPath()
        {
            return @$"{GetTempPath()}monitors/";
        }
        /// <summary>
        /// Gets the path to the vBenchSLAM in user's documents folder
        /// </summary>
        /// <returns></returns>
        public static string GetUserDocumentsFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "vBenchSLAM");
        }

        /// <summary>
        /// Copies the content and the structure of the whole sub directory
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