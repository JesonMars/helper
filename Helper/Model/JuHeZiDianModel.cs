using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper.Model 
{
    public class JuHeZiDianModel
    {
        public string reason { get; set; }
        public string error_code { get; set; }
        public JuHeZiDianResult result { get; set; }

        public class JuHeZiDianResult
        {
            public string id { get; set; }
            public string zi { get; set; }
            public string py { get; set; }
            public string wubi { get; set; }
            public string bushou { get; set; }
            public string pinyin { get; set; }
            public string bihua { get; set; }
        }
    }

    
}
