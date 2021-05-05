namespace vBenchSLAM.Addins.Abstract
{
    public interface ICsvParsable
    {
        string GetCsvHeaderRow();
        string ParseAsCsvLiteral();
    }
}