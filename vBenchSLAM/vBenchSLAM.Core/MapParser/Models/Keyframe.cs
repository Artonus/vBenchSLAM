using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.MapParser.Models
{
    public class Keyframe
    {
        public int src_frm_id { get; set; }
        /// <summary>
        /// timestamp
        /// </summary>
        public double ts { get; set; }

        public string cam { get; set; }
        public float depth_thr { get; set; }
        public double[] root_cw { get; set; }
        public double[] trans_cw { get; set; }
        public int n_keypts { get; set; }
        public List<KeyPoint> keypts { get; set; }
        public List<KeyPoint> undists { get; set; }
        public float[] x_rights { get; set; }
        public int[] depths { get; set; }

        public List<int[]> descs { get; set; }
        public int[] ln_ids { get; set; }
        public uint n_scale_levels { get; set; }
        public float scale_factor { get; set; }
        public int span_parent { get; set; }
        public int[] span_children { get; set; }
        public int[] loop_edges { get; set; }
    }
}
