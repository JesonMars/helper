using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper.Model
{
    public class YouDaoTransModel
    {
        public List<string> translation { get; set; }

        public string query { get; set; }

        public string errorCode { get; set; }

        public List<WebTrans> web { get; set; } 
    }

    public class WebTrans
    {
        public string key { get; set; }
        public List<string> value { get; set; } 
    }
}
