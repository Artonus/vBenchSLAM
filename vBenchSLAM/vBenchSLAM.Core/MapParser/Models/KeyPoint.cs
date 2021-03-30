using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.MapParser.Models
{
    public class KeyPoint
    {
        public double ang { get; set; }
        public short oct { get; set; }
        public int[] pt { get; set; }
    }
}
