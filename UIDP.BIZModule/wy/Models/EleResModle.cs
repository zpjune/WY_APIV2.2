using System;
using System.Collections.Generic;
using System.Text;

namespace UIDP.BIZModule.wy.Models
{
    public class EleResModle
    {
        public string opr_id { get; set; }
        public string resolve_time { get; set; }
        public string status { get; set; }
        public List<ResModel> data { get; set; }
        public string error_msg { get; set; }

        /* [{
        "opr_id": "e3dfb115-2f07-46eb-a36f-0b6442bb1d1e",
        "resolve_time": "2020-03-27 17:14:02",
        "status": "SUCCESS",
        "data": [{
            "type": 3,
            "value": ["0.00", "0.00", "0.00", "0.00", "0.00"],
            "dsp": "总 : 0.00 kWh 尖 : 0.00 kWh 峰 : 0.00 kWh 平 : 0.00 kWh 谷 : 0.00 kWh"
        }]
    }]*/
    }
    public class ResModel{
       public string type { get; set; }
        public string[] value { get; set; }
        public string dsp { get; set; }
    }
}
