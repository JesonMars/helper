using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Helper
{
    public class MathHelper
    {
        public static decimal Parse(string str)
        {
            var val = Regex.Match(str, @"[0-9]{1,}.[0-9]{1,}|[0-9]{1,}").Value;
            val = string.IsNullOrEmpty(val) ?"0" : val;
            return decimal.Parse(val);
        }
    }
}
