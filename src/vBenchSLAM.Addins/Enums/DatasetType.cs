using vBenchSLAM.Addins.Attributes;

namespace vBenchSLAM.Addins
{
    /// <summary>
    /// Represents the supported types of the dataset
    /// </summary>
    public enum DatasetType
    {
        /// <summary>
        /// KITTY dataset
        /// </summary>
        [StringValue("KITTY")]
        Kitty,
        /// <summary>
        /// Custom dataset
        /// </summary>
        [StringValue("Other")]
        Other
    }
}