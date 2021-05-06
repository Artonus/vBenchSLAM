using vBenchSLAM.Addins.Attributes;

namespace vBenchSLAM.Addins
{
    /// <summary>
    /// Represents the available mapping frameworks
    /// </summary>
    public enum MapperType
    {
        /// <summary>
        /// OpenVSLAM framework
        /// </summary>
        [StringValue("OpenVSLAM")]
        OpenVslam,
        /// <summary>
        /// ORB_SLAM2 framework
        /// </summary>
        [StringValue("ORB_SLAM2")]
        OrbSlam
    }
}
