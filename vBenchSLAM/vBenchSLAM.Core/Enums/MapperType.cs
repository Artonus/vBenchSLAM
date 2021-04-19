using vBenchSLAM.Addins.Attributes;

namespace vBenchSLAM.Core.Enums
{
    public enum MapperType
    {
        [StringValue("OpenVSLAM")]
        OpenVslam,
        [StringValue("ORBSLAM2")]
        OrbSlam
    }
}
