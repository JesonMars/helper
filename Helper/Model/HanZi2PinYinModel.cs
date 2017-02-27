using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper.Model
{
    public class HanZi2PinYinModel
    {
        public string showapi_res_code { get; set; }
        public string showapi_res_error { get; set; }
        public ResBody showapi_res_body { get; set; }
    }

    public class ResBody {
        public string data { get; set; }
        public string simpleData { get; set; }
        public bool flag { get; set; }
    }
}
