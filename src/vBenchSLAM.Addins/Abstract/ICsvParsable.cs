namespace vBenchSLAM.Addins.Abstract
{
    public interface ICsvParsable
    {
        /// <summary>
        /// Get the header row for the CSV file
        /// </summary>
        /// <returns></returns>
        string GetCsvHeaderRow();
        /// <summary>
        /// Parse to the CSV string
        /// </summary>
        /// <returns></returns>
        string ParseAsCsvLiteral();
    }
}