using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace vBenchSLAM.Core.MapParser.Models
{
    [MessagePackObject]
    public class Map
    {
        [Key(0)]
        public List<Camera> Cameras { get; set; }
        public int frame_next_id { get; set; }
        public int keyframe_next_id { get; set; }
        public List<Keyframe> keyframes { get; set; }
    }
}
