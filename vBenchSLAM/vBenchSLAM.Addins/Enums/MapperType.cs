using vBenchSLAM.Addins.Attributes;

namespace vBenchSLAM.Addins
{
    public enum MapperType
    {
        [StringValue("OpenVSLAM")]
        OpenVslam,
        [StringValue("ORB_SLAM2")]
        OrbSlam
    }
}
