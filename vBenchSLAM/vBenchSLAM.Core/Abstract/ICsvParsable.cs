using Docker.DotNet.Models;

namespace vBenchSLAM.Core
{
    public interface ICsvParsable
    {
        string GetCsvHeaderRow();
        string ParseAsCsvLiteral();
    }
}