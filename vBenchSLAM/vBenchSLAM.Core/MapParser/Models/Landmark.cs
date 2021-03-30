using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vBenchSLAM.Addins.Models;

namespace vBenchSLAM.Core.MapParser.Models
{
    public class Landmark
    {
        public int st_keyfrm { get; set; }
        public int n_fnd { get; set; }
        public int n_vis { get; set; }
        public Vector pos_w { get; set; }
    }
}
