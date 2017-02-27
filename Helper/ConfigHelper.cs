using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Helper
{
    public class ConfigHelper
    {
        private static string GetAppSetting(string key)
        {
            var result=ConfigurationSettings.AppSettings.Get(key);
            return string.IsNullOrEmpty(result) ? "" : result;
        }
        public static string GetBusinessSuffix()
        {
            return GetAppSetting("BusinessSuffix");
        }

        public static string GetDalSuffix()
        {
            return GetAppSetting("DalSuffix");
        }

        public static string GetEntitySuffix()
        {
            return GetAppSetting("EntitySuffix");
        }

        public static string GetYouDaoUrl()
        {
            var youdaourl = GetAppSetting("youdaourl");
            return youdaourl.Replace("|", "&");
        }

        public static string GetExcelExtesion()
        {
            var exten=GetAppSetting("exceltype");
            return exten;
        }

        public static string GetDestFileName()
        {
            var name =new StringBuilder(GetAppSetting("destfilename"));
            name.Append(DateTime.Now.ToString("Mddyyyy"));
            return name.ToString();
        }
        public static string GetPostCodeRegex()
        {
            var name = new StringBuilder(GetAppSetting("postcoderegex"));
            return name.ToString();
        }

        public static List<string> GetDestFileHead()
        {
            var name = GetAppSetting("destfilehead");
            return name.Split(',').ToList();
        }

        public static int GetPostCodeHeadCount()
        {
            var name = GetAppSetting("postcodeheadcount");
            return int.Parse(name);
        }
        public static string GetJuHeZiDianUrl()
        {
            var juhezidian = GetAppSetting("juhezidian");
            return juhezidian.Replace("|", "&");
        }
        public static string GetCounty() {
            var juhezidian = GetAppSetting("county");
            return juhezidian.Replace("'", "\"");
        }
        public static string GetFanYiDics()
        {
            var juhezidian = GetAppSetting("fanyidics");
            return juhezidian.Replace("'", "\"");
        }
        public static string GetHanZi2PinYinUrl()
        {
            var juhezidian = GetAppSetting("hanzi2pinyin");
            return juhezidian.Replace("|", "&");
        }
    }
}
