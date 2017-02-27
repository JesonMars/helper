using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;

namespace Helper
{
    /// <summary>
    /// HTTP请求相关的封装工具类
    /// </summary>
    public class HttpHelper
    {
        private static int defaultTimeout = 5000;

        public static int DefaultTimeout
        {
            get
            {
                return defaultTimeout;
            }
            set 
            {
                if (value > 0)
                {
                    defaultTimeout = value;
                }
            }
        }


        /// <summary>
        /// 根据指定的编码格式返回HttpRequest请求的参数集合，用户在服务端获得客户端请求的HttpRequest请求参数，
        /// Get方式通过QuqueryString获取，Post方式则通过流获取，返回的结构为NameValueCollection集合；
        /// </summary>
        /// <param name="request">请求的字符串</param>
        /// <param name="encode">编码模式</param>
        /// <returns>NameValueCollection参数集合</returns>
        public static NameValueCollection GetRequestParameters(HttpRequest request, string encode)
        {
            Encoding destEncode = null;
            if (!String.IsNullOrEmpty(encode))
            {
                try
                {
                    destEncode = Encoding.GetEncoding(encode);
                }
                catch { }
            }

            return GetRequestParameters(request, destEncode);
        }

        /// <summary>
        /// 根据指定的编码格式返回HttpRequest请求的参数集合，用户在服务端获得客户端请求的HttpRequest请求参数，
        /// Get方式通过QuqueryString获取，Post方式则通过流获取，返回的结构为NameValueCollection集合；
        /// </summary>
        /// <param name="request">请求的字符串</param>
        /// <param name="encode">编码模式</param>
        /// <returns>NameValueCollection参数集合</returns>
        public static NameValueCollection GetRequestParameters(HttpRequest request, Encoding encode)
        {
            NameValueCollection nv = null;
            if (request.HttpMethod == "POST")
            {
                if (null != encode)
                {
                    Stream resStream = request.InputStream;
                    byte[] filecontent = new byte[resStream.Length];
                    resStream.Read(filecontent, 0, filecontent.Length);
                    string postquery = Encoding.Default.GetString(filecontent);
                    nv = HttpUtility.ParseQueryString(postquery, encode);
                }
                else
                    nv = request.Form;
            }
            else
            {
                if (null != encode)
                {
                    nv = System.Web.HttpUtility.ParseQueryString(request.Url.Query, encode);
                }
                else
                {
                    nv = request.QueryString;
                }
            }
            return nv;
        }


        /// <summary>
        /// 发送一个POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paras"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static byte[] SendPostRequest(string url, string paras, string encodetype)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "post";
            req.ContentType = "application/x-www-form-urlencoded";
            ConfigHttpWebRequest(req, defaultTimeout);

            Encoding encode = Encoding.Default;
            if (!String.IsNullOrEmpty(encodetype))
            {
                try
                {
                    encode = Encoding.GetEncoding(encodetype);
                }
                catch { }
            }

            byte[] data = encode.GetBytes(paras.ToString());
            Stream reqstream = req.GetRequestStream();

            reqstream.Write(data, 0, data.Length);
            reqstream.Close();

            HttpWebResponse res = null;
            try
            {
                res = (HttpWebResponse)req.GetResponse();
                Stream resst = res.GetResponseStream();
                byte[] result = new byte[8092];
                resst.Read(result, 0, result.Length);
                resst.Close();
                return result;
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] SendGetRequest(string url, string encodetype)
        {
            Encoding encode = Encoding.Default;
            if (!String.IsNullOrEmpty(encodetype))
            {
                try
                {
                    encode = Encoding.GetEncoding(encodetype);
                }
                catch { }
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.MaximumAutomaticRedirections = 3;
            ConfigHttpWebRequest(req, defaultTimeout);

            HttpWebResponse res = null;
            try
            {
                res = (HttpWebResponse)req.GetResponse();
                Stream resst = res.GetResponseStream();
                byte[] result = new byte[8096];
                resst.Read(result, 0, result.Length);
                resst.Close();
                return result;
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
        }

        /// <summary>
        /// 使用指定编码格式发送一个POST请求，并通过约定的编码格式获取返回的数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="parameters">请求的参数集合</param>
        /// <param name="reqencode">请求的编码格式</param>
        /// <param name="resencode">接收的编码格式</param>
        /// <returns></returns>
        public static string SendPostRequest(string url, NameValueCollection parameters, Encoding reqencode, Encoding resencode)
        {
            return SendPostRequest(url, parameters, reqencode, resencode, defaultTimeout);
        }


        /// <summary>
        /// 使用指定编码格式发送一个POST请求，并通过约定的编码格式获取返回的数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="parameters">请求的参数集合</param>
        /// <param name="reqencode">请求的编码格式</param>
        /// <param name="resencode">接收的编码格式</param>
        /// <param name="timeOut">超时时间 (毫秒)</param>
        /// <returns></returns>
        public static string SendPostRequest(string url, NameValueCollection parameters, Encoding reqencode, Encoding resencode, int timeOut)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "post";
            req.ContentType = "application/x-www-form-urlencoded";
            ConfigHttpWebRequest(req, timeOut);

            StringBuilder parassb = new StringBuilder();
            if (null != parameters)
            {
                foreach (string key in parameters.Keys)
                {
                    if (parassb.Length > 0)
                        parassb.Append("&");
                    parassb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, reqencode), HttpUtility.UrlEncode(parameters[key], reqencode));
                }
            }
            byte[] data = reqencode.GetBytes(parassb.ToString());
            Stream reqstream = req.GetRequestStream();

            reqstream.Write(data, 0, data.Length);
            reqstream.Close();

            HttpWebResponse res = null;
            try
            {
                res = (HttpWebResponse)req.GetResponse();
                string result = String.Empty;
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), resencode))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
        }


        /// <summary>
        /// 通过客户端跳转发起Post请求并且重定向
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        public static void PostAndRedirect(string url, NameValueCollection parameters, HttpContext context)
        {
            StringBuilder script = new StringBuilder();
            script.AppendFormat("<form name=redirpostform action='{0}' method='post'>", url);
            if (null != parameters)
            {
                foreach (string key in parameters.Keys)
                {
                    script.AppendFormat("<input type='hidden' name='{0}' value='{1}'>",
                        key, parameters[key]);
                }
            }
            script.Append("</form>");
            script.Append("<script language='javascript'>redirpostform.submit();</script>");
            context.Response.Write(script);
            context.Response.End();
        }

        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="parameters">请求的参数集合</param>
        /// <param name="reqencode">请求的编码格式</param>
        /// <param name="resencode">接收的编码格式</param>
        /// <returns></returns>
        public static string SendGetRequest(string baseurl, NameValueCollection parameters, Encoding reqencode, Encoding resencode)
        {
            return SendGetRequest(baseurl, parameters, reqencode, resencode, defaultTimeout);
        }

        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <param name="parameters">请求的参数集合</param>
        /// <param name="reqencode">请求的编码格式</param>
        /// <param name="resencode">接收的编码格式</param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static string SendGetRequest(string baseurl, NameValueCollection parameters, Encoding reqencode,
                                            Encoding resencode, int timeOut)
        {
            StringBuilder parassb = new StringBuilder();
            if (null != parameters)
            {
                foreach (string key in parameters.Keys)
                {
                    if (parassb.Length > 0)
                        parassb.Append("&");
                    parassb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, reqencode), HttpUtility.UrlEncode(parameters[key], reqencode));
                }
            }
            if (parassb.Length > 0)
            {
                baseurl += "?" + parassb;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(baseurl);
            req.Method = "GET";
            req.MaximumAutomaticRedirections = 3;
            ConfigHttpWebRequest(req, timeOut);

            string result = String.Empty;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)req.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), resencode))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }


        public static string BuilderGetRequestUrl(string baseurl, NameValueCollection parameters, Encoding reqencode)
        {
            StringBuilder parassb = new StringBuilder();
            if (null != parameters)
            {
                foreach (string key in parameters.Keys)
                {
                    if (parassb.Length > 0)
                        parassb.Append("&");
                    parassb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, reqencode), HttpUtility.UrlEncode(parameters[key], reqencode));
                }
            }

            if (parassb.Length > 0)
            {
                baseurl += "?" + parassb;
            }
            return baseurl;
        }

        public static string BuilderGetRequestUrlNoEncode(string baseurl, NameValueCollection parameters)
        {
            StringBuilder parassb = new StringBuilder();
            if (null != parameters)
            {
                foreach (string key in parameters.Keys)
                {
                    if (parassb.Length > 0)
                        parassb.Append("&");
                    parassb.AppendFormat("{0}={1}", key, parameters[key]);
                }
            }

            if (parassb.Length > 0)
            {
                baseurl += "?" + parassb;
            }
            return baseurl;
        }
        /// <summary>
        /// 转换输入字符串的编码格式
        /// </summary>
        /// <param name="input"></param>
        /// <param name="srcEncoding"></param>
        /// <param name="desEncoding"></param>
        /// <returns></returns>
        public static string EncodeString(string input, Encoding srcEncoding, Encoding desEncoding)
        {
            if (srcEncoding == null || desEncoding == null)
            {
                throw new Exception("需要提供相应的encoding");
            }
            if (srcEncoding == desEncoding)
            {
                return input;
            }
            else
            {
                return desEncoding.GetString(Encoding.Convert(srcEncoding, desEncoding, srcEncoding.GetBytes(input)));
            }
        }

        /// <summary>
        /// 转换输入字符串的编码格式
        /// </summary>
        /// <param name="input"></param>
        /// <param name="srcEncoding"></param>
        /// <param name="desEncoding"></param>
        /// <returns></returns>
        public static string EncodeString(string input, Encoding desEncoding)
        {
            if (desEncoding == null)
            {
                throw new Exception("需要提供相应的encoding");
            }
            if (Encoding.Default == desEncoding)
            {
                return input;
            }
            else
            {
                return desEncoding.GetString(Encoding.Convert(Encoding.Default, desEncoding, Encoding.Default.GetBytes(input)));
            }
        }

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <returns>服务器响应</returns>
        public static string PostJsonDataToUrl(string data, string url)
        {
            string sRequestEncoding = "utf-8";
            Encoding encoding = Encoding.GetEncoding(sRequestEncoding);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostJsonDataToUrl(bytesToPost, url);
        }

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <returns>服务器响应</returns>
        public static string PostJsonDataToUrl(byte[] data, string url)
        {
            #region 创建httpWebRequest对象

            string sUserAgent =
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            string sContentType =
                "application/json;charset=utf-8";
            string sResponseEncoding = "utf-8";

            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(
                    string.Format("Invalid url string: {0}", url)
                    );
            }
            #endregion

            #region 填充httpWebRequest的基本信息
            httpRequest.UserAgent = sUserAgent;
            httpRequest.ContentType = sContentType;
            httpRequest.Method = "POST";
            ConfigHttpWebRequest(httpRequest, defaultTimeout);
            #endregion

            #region 填充要post的内容
            httpRequest.ContentLength = data.Length;
            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            #endregion

            #region 发送post请求到服务器并读取服务器返回信息
            HttpWebResponse res = null;
            try
            {
                res = (HttpWebResponse)httpRequest.GetResponse();
                string stringResponse = string.Empty;
                using (StreamReader responseReader =
                    new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(sResponseEncoding)))
                {
                    stringResponse = responseReader.ReadToEnd();
                }
                return stringResponse;
            }
            catch (Exception e)
            {
                // log error
                Console.WriteLine(
                    string.Format("POST操作发生异常：{0}", e.Message)
                    );
                throw e;
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
            #endregion
        }

        /// <summary>
        /// Post data到url
        /// </summary>
        /// <param name="data">要post的数据</param>
        /// <param name="url">目标url</param>
        /// <param name="timeout"></param>
        /// <returns>服务器响应</returns>
        public static string PostJsonDataToUrl(string data, string url, int timeout, Dictionary<string, string> headers = null)
        {
            string sRequestEncoding = "utf-8";
            Encoding encoding = Encoding.GetEncoding(sRequestEncoding);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostJsonDataToUrl(bytesToPost, url, timeout, headers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string PostJsonDataToUrl(byte[] data, string url, int timeout, Dictionary<string,string> headers = null)
        {
            #region 创建httpWebRequest对象

            string sUserAgent =
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            string sContentType =
                "application/json;charset=utf-8";
            string sResponseEncoding = "utf-8";

            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (timeout > 0)
            {
                ConfigHttpWebRequest(httpRequest, timeout);
            }
            if (httpRequest == null)
            {
                throw new ApplicationException(
                    string.Format("Invalid url string: {0}", url)
                    );
            }
            #endregion

            #region 填充httpWebRequest的基本信息
            httpRequest.UserAgent = sUserAgent;
            httpRequest.ContentType = sContentType;
            httpRequest.Method = "POST";
            
            if (headers != null && headers.Count > 0)
            {
                foreach (var kv in headers)
                {
                    httpRequest.Headers.Add(kv.Key, kv.Value);
                }
            }
            #endregion

            #region 填充要post的内容
            httpRequest.ContentLength = data.Length;
            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            #endregion

            #region 发送post请求到服务器并读取服务器返回信息
            HttpWebResponse res = null;
            try
            {
                res = (HttpWebResponse)httpRequest.GetResponse();
                string stringResponse = string.Empty;
                using (StreamReader responseReader =
                    new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(sResponseEncoding)))
                {
                    stringResponse = responseReader.ReadToEnd();
                }
                return stringResponse;
            }
            catch (Exception e)
            {
                // log error
                Console.WriteLine(
                    string.Format("POST操作发生异常：{0}", e.Message)
                    );
                throw e;
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
            #endregion
        }

        private static void ConfigHttpWebRequest(HttpWebRequest req, int timeout)
        {
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
            req.KeepAlive = false;
            req.Proxy = null;
        }

        /// <summary>
        /// httpwebrequest帮助类
        /// </summary>
        /// <example>var response = HttpHelper.SendRequest("http://www.baidu.com", "get"
        ///         , new { appkey = appkey, accesstoken = access_token }
        ///         , (req) => {
        ///             req.Accept = "application/json";
        ///         });
        /// </example>
        /// <param name="method">post或get</param>
        /// <param name="param">提交参数</param>
        /// <param name="reqSetFunc">请求之前对webrequest对象的修改，常用:
        ///     req.Referer = "http://passport.csdn.net";
        ///     req.Accept = "application/json";
        ///     req.Headers.Add("X-ACL-TOKEN", _token);        
        ///     req.MaximumAutomaticRedirections = 3;
        ///     req.Timeout = 5000;
        /// </param>        
        /// <param name="encoding">编码，默认为utf8</param>
        public static ResponseInfo SendQueryRequest(string url, string method
            , NameValueCollection param = null, Action<HttpWebRequest> reqSetFunc = null,            
            Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            #region 设置url
            if (method.Equals("get", StringComparison.CurrentCultureIgnoreCase)
                && param != null)
            {
                StringBuilder urlbuilder = new StringBuilder();
                foreach (var key in param.AllKeys)
                {
                    if (urlbuilder.Length > 0)
                        urlbuilder.Append("&");
                    urlbuilder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, encoding), HttpUtility.UrlEncode(param[key], encoding));
                }
                if (url.Contains("?"))
                {
                    url += "&" + urlbuilder.ToString();
                }
                else
                {
                    url += "?" + urlbuilder.ToString();
                }
                param = null;
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.ContentType = "application/x-www-form-urlencoded";
            req.MaximumAutomaticRedirections = 3;
            req.Timeout = 5000;

            if (reqSetFunc != null)
            {
                reqSetFunc(req);
            }
            #endregion

            WebResponse res;
            #region 处理参数
            if (param != null)
            {
                string reqdata = string.Empty;
                
                StringBuilder reqdatabuilder = new StringBuilder();
                foreach (string key in param.AllKeys)
                {
                    if (reqdatabuilder.Length > 0)
                        reqdatabuilder.Append("&");
                    reqdatabuilder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key, encoding), HttpUtility.UrlEncode(param[key], encoding));
                }
                reqdata = reqdatabuilder.ToString();

                byte[] data = encoding.GetBytes(reqdata);

                using (Stream reqstream = req.GetRequestStream())
                {
                    reqstream.Write(data, 0, data.Length);
                    reqstream.Close();
                }
            }
            #endregion
            
            try
            {
                res = req.GetResponse();                
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw ex;
                }
                res = ex.Response;
            }

            return new ResponseInfo(res, encoding);
        }

        /// <summary>
        /// httpwebrequest帮助类
        /// </summary>
        /// <example>var response = HttpHelper.SendRequest("http://www.baidu.com", "get"
        ///         , new { appkey = appkey, accesstoken = access_token }
        ///         , (req) => {
        ///             req.Accept = "application/json";
        ///         });
        /// </example>
        /// <param name="method">post或get</param>
        /// <param name="param">提交参数</param>
        /// <param name="reqSetFunc">请求之前对webrequest对象的修改，常用:
        ///     req.Referer = "http://passport.csdn.net";
        ///     req.Accept = "application/json";
        ///     req.Headers.Add("X-ACL-TOKEN", _token);        
        ///     req.MaximumAutomaticRedirections = 3;
        ///     req.Timeout = 5000;
        /// </param>        
        /// <param name="encoding">编码，默认为utf8</param>
        public static ResponseInfo SendJsonRequest<T>(string url, string method
            , T param, Action<HttpWebRequest> reqSetFunc = null,            
            Encoding encoding = null, Func<T, string> serializeFunc = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            #region 设置url
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.ContentType = "application/json";
            req.MaximumAutomaticRedirections = 3;
            req.Timeout = 5000;

            if (reqSetFunc != null)
            {
                reqSetFunc(req);
            }
            #endregion

            WebResponse res;
            #region 处理参数
            if (param != null)
            {
                string reqdata = string.Empty;
                if (serializeFunc != null)
                {
                    reqdata = serializeFunc(param);
                }
                else
                {
                    var jsonhelper = new JsonHelper();
                    reqdata = jsonhelper.JsonSerializer<T>(param);
                }

                byte[] data = encoding.GetBytes(reqdata);                
                using (Stream reqstream = req.GetRequestStream())
                {
                    reqstream.Write(data, 0, data.Length);
                    reqstream.Close();
                }
            }
            #endregion

            try
            {
                res = req.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    throw ex;
                }
                res = ex.Response;
            }

            return new ResponseInfo(res, encoding);
        }

        public class ResponseInfo
        {
            public ResponseInfo(WebResponse response, Encoding encoding)
            {
                this._response = response; 
                this._encoding = encoding;
            }

            private WebResponse _response { get; set; }
            private Encoding _encoding{get;set;}
            private string _contentString = string.Empty;

            public HttpStatusCode HttpStatus {
                get { 
                    if(this._response == null)
                    {
                        return HttpStatusCode.BadRequest;
                    }
                    return ((System.Net.HttpWebResponse)this._response).StatusCode;
                }
            }
            public string ContentString {
                get 
                {
                    if (this._response == null)
                    {
                        return string.Empty;
                    }

                    if (!string.IsNullOrEmpty(this._contentString))
                    {
                        return this._contentString;
                    }

                    if (this._encoding == null)
                    {
                        this._encoding = Encoding.UTF8;
                    }

                    using (StreamReader reader = new StreamReader(this._response.GetResponseStream(), this._encoding))
                    {
                        this._contentString = reader.ReadToEnd();
                        return this._contentString;
                    }
                }                
            }
        }

        public static string UrlEncode(string input, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return HttpUtility.UrlEncode(input, encoding);
        }

        public static string UrlDecode(string input, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return HttpUtility.UrlDecode(input, encoding);
        }

        public NameValueCollection ParseQueryString(string input, Encoding encoding = null)
        {
            if(encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return HttpUtility.ParseQueryString(input, encoding);
        }
    }
}
