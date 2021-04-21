using vBenchSLAM.Addins.Attributes;

namespace vBenchSLAM.Core.Enums
{
    public enum MapperType
    {
        [StringValue("OpenVSLAM")]
        OpenVslam,
        [StringValue("ORB_SLAM2")]
        OrbSlam
    }
}
