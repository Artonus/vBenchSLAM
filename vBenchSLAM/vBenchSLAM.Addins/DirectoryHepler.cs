using System.IO;

namespace vBenchSLAM.Addins
{
    public static class DirectoryHepler
    {
        public static string GetTempPath()
        {
            return @$"{Path.GetTempPath()}/vBenchSLAM";
        }
    }
}