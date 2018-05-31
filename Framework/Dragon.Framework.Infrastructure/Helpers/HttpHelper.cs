using Dragon.Framework.Infrastructure.HttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Dragon.Framework.Infrastructure.Helpers
{
    /// <summary>
    /// HTTP请求帮助类
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="requestParameter">请求报文</param>
        /// <returns>响应报文</returns>
        public static HttpResponseParameter Excute(HttpRequestParameter requestParameter)
        {
            SetParameter(null, requestParameter);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(requestParameter.Uri, UriKind.RelativeOrAbsolute));
            SetHeader(webRequest, requestParameter);
            SetCookie(webRequest, requestParameter);

            // https请求设置
            if (Regex.IsMatch(requestParameter.Uri, "^https://"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            }

            SetParameter(webRequest, requestParameter);
            return SetResponse(webRequest, requestParameter);
        }

        #region 公共方法

        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="requestParameter">请求参数对象</param>
        private static void SetHeader(HttpWebRequest request, HttpRequestParameter requestParameter)
        {
            request.Method = requestParameter.RequestType.ToString();
            request.ContentType = "application/json";
        }

        /// <summary>
        /// 设置请求Cookie
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="requestParameter">请求参数对象</param>
        private static void SetCookie(HttpWebRequest request, HttpRequestParameter requestParameter)
        {
            request.CookieContainer = new CookieContainer();
            if (requestParameter.Cookie != null && !string.IsNullOrEmpty(requestParameter.Cookie.Cookie))
            {
                request.Headers[HttpRequestHeader.Cookie] = requestParameter.Cookie.Cookie;
            }

            if (requestParameter.Cookie != null && requestParameter.Cookie.Cookies != null && requestParameter.Cookie.Cookies.Count > 0)
            {
                request.CookieContainer.Add(requestParameter.Cookie.Cookies);
            }
        }

        /// <summary>
        /// 设置请求参数
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="requestParameter">请求参数对象</param>
        private static void SetParameter(HttpWebRequest request, HttpRequestParameter requestParameter)
        {
            if (requestParameter.RequestType == RequestType.GET)
            {
                if (request != null)
                {
                    return;
                }

                //Get 拼接URL参数

                if (requestParameter.Parameters == null || requestParameter.Parameters.Count <= 0)
                {
                    return;
                }

                StringBuilder requestParamsBuilder = new StringBuilder();
                foreach (KeyValuePair<string, string> keyValuePair in requestParameter.Parameters)
                {
                    requestParamsBuilder.AppendFormat("{0}={1}&", keyValuePair.Key, keyValuePair.Value);
                }

                requestParameter.Uri = string.Concat(requestParameter.Uri, $"?{requestParamsBuilder.ToString().TrimEnd('&')}");
            }
            else if (request != null)
            {
                //Post 设置请求参数

                string requestData;
                if (requestParameter.Parameters == null || requestParameter.Parameters.Count <= 0)
                {
                    requestData = "{}";
                }
                else
                {
                    requestData = JsonHelper.SerializeObject(requestParameter.Parameters);
                }

                byte[] bytePosts = requestParameter.Encoding.GetBytes(requestData);
                request.ContentLength = bytePosts.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytePosts, 0, bytePosts.Length);
                }
            }
        }

        /// <summary>
        /// 设置返回结果
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="requestParameter">请求参数对象</param>
        /// <returns>响应对象</returns>
        private static HttpResponseParameter SetResponse(HttpWebRequest request, HttpRequestParameter requestParameter)
        {
            HttpResponseParameter responseParameter = new HttpResponseParameter();
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                responseParameter.Uri = webResponse.ResponseUri;
                responseParameter.StatusCode = webResponse.StatusCode;
                responseParameter.Cookie = new HttpCookieType
                {
                    Cookies = webResponse.Cookies,
                    Cookie = webResponse.Headers["Set-Cookie"]
                };

                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), requestParameter.Encoding))
                {
                    responseParameter.Body = reader.ReadToEnd();
                }
            }

            return responseParameter;
        }

        /// <summary>
        /// ssl/https请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        #endregion
    }
}
