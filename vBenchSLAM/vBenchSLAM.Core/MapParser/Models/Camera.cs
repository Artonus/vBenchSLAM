using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vBenchSLAM.Core.MapParser.Models
{
    public class Camera
    {
        public string color_order { get; set; }
        public int cols { get; set; }
        public int  focal_x_baseline { get; set; }
        public int fps { get; set; }
        public string model_type { get; set; }
        public int num_grid_cols { get; set; }
        public int num_grid_rows { get; set; }
        public int rows { get; set; }
        public string setup_type { get; set; }
    }
}
