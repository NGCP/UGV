using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGCP.UGV
{
    public class Payload
    {
        #region Properties
        public bool is_payload { get; set; }
        public double dist_appr { get; set; }
        public int x_coor { get; set; }
        public int y_coor { get; set; }

        #endregion Properties

        public Payload()
        {
            is_payload = false;
            dist_appr = 50;
            x_coor = 0;
            y_coor = 0;
        }
    }
}
