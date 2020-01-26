using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MagneticCrawler
{
        public class RequestParameter
        {
                public string Url { get; set; }
                public string HttpMethod { get; set; }
                public string UserAgent { get; set; }
                public Encoding Encoding { get; set; }
                public RequestParameter()
                {
                        HttpMethod = "GET";
                        Encoding = Encoding.UTF8;
                }
        }
        public static class WebRequestHelper
        {
                public static string Request(RequestParameter requestParameter)
                {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestParameter.Url);
                        request.Method = requestParameter.HttpMethod;

                        if (!string.IsNullOrEmpty(requestParameter.UserAgent))
                        {
                                request.UserAgent = requestParameter.UserAgent;
                        }

                        string invokeresult = string.Empty;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                                using (StreamReader reader = new StreamReader(response.GetResponseStream(), requestParameter.Encoding))
                                {
                                        invokeresult = reader.ReadToEnd();
                                }
                        }
                        return invokeresult;
                }

                public static string Request(string url)
                {
                        RequestParameter parameter = new RequestParameter
                        {
                                Url = url
                        };

                        return Request(parameter);
                }

                private class ResuestParam
                {
                        public HttpWebRequest Request
                        {
                                get;
                                set;
                        }
                        public Action<object, string> OnSuccess
                        {
                                get;
                                set;
                        }

                        public Action<object, string> OnError
                        {
                                get;
                                set;
                        }

                        public object Param
                        {
                                get;
                                set;
                        }
                }

                public static void RequestAsync(string url, Action<object, string> successaction, Action<object, string> erraction, object param)
                {
                        RequestAsync(url, null, successaction, erraction, param);
                }

                public static void RequestAsync(string url, string useragent, Action<object, string> successaction, Action<object, string> erraction, object param)
                {
                        try
                        {
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                request.Method = "GET";

                                if (!string.IsNullOrEmpty(useragent))
                                {
                                        request.UserAgent = useragent;
                                }

                                string invokeresult = string.Empty;

                                request.BeginGetResponse(GetResponseCallBack, new ResuestParam()
                                {
                                        Request = request,
                                        OnSuccess = successaction,
                                        OnError = erraction,
                                        Param = param
                                });
                        }
                        catch (Exception e)
                        {
                                erraction?.Invoke(param, string.Empty);
                        }
                }

                private async static void GetResponseCallBack(IAsyncResult ar)
                {
                        ResuestParam param = (ResuestParam)ar.AsyncState;
                        try
                        {
                                string requeststr = string.Empty;

                                using (HttpWebResponse response = (HttpWebResponse)param.Request.EndGetResponse(ar))
                                {
                                        using (Stream stream = response.GetResponseStream())
                                        {
                                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                                {
                                                        requeststr = await reader.ReadToEndAsync();
                                                }
                                        }
                                }
                                param.OnSuccess?.Invoke(param.Param, requeststr);
                        }
                        catch (Exception e)
                        {
                                param.OnError?.Invoke(null, string.Empty);
                        }
                }
        }
}
