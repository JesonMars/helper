using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Helper
{
    public class CityHelper
    {
        /// <summary>
        /// 是否是直辖市
        /// </summary>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <returns>true，属于直辖市；false，不属于直辖市</returns>
        public static bool IsMunicipality(string province,string city)
        {
            return city.Contains(province);
        }

        /// <summary>
        /// 是否是直辖市
        /// </summary>
        /// <param name="province">省份</param>
        /// <param name="city">城市</param>
        /// <returns>true，属于直辖市；false，不属于直辖市</returns>
        public static string GetAddress(string address)
        {
            var result = "";
            var addressNew = Regex.Replace(address, @"\([0-9]*\)", "");
            var matches = Regex.Match(addressNew, @"(\w{1,}\s){3}").Value;
            if (string.IsNullOrEmpty(matches))
            {
                matches = Regex.Match(addressNew, @"(\w{1,}\s){2}").Value;
            }
            if (string.IsNullOrEmpty(matches))
            {
                matches = Regex.Match(addressNew, @"(\w{1,}\s){1}").Value;
            }
            
            try
            {
                result = Regex.Replace(addressNew.Replace(matches, ""), @"\s", "");
            }
            catch (Exception ex)
            {
                result = addressNew;
            }
            return result;
        }
    }
}
