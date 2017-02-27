using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace Helper
{
    public class JsonHelper
    {
        private Encoding _Encoding;

        public JsonHelper()
        {
            _Encoding = Encoding.UTF8;
        }

        public JsonHelper(Encoding e)
        {
            _Encoding = e;
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        public string JsonSerializer<T>(T t)
        {
            if (t == null)
            {
                return string.Empty;
            }

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = _Encoding.GetString(ms.ToArray());
            ms.Close();

            //替换Json的Date字符串
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public T JsonDeserialize<T>(string jsonString)
        {
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(_Encoding.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public T JsonDeserializeNoTime<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(_Encoding.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// NewtonJSON序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string NewtonsoftSerializeObject(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                Newtonsoft.Json.Converters.IsoDateTimeConverter converter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                converter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                return Newtonsoft.Json.JavaScriptConvert.SerializeObject(obj, converter);
            }
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public T DeserializeJsonToObject<T>(string json) where T : class
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new Newtonsoft.Json.JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        public string JsonSerializer(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, obj);
            string jsonString = _Encoding.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public object JsonDeserialize(string jsonString, Type t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(t);
            MemoryStream ms = new MemoryStream(_Encoding.GetBytes(jsonString));
            object obj = ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
    }
}
