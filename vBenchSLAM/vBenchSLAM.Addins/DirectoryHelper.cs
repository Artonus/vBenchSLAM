using System.IO;

namespace vBenchSLAM.Addins
{
    public static class DirectoryHelper
    {
        public static string GetTempPath()
        {
            return @$"{Path.GetTempPath()}vBenchSLAM/";
        }
    }
}