using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SisConAxs.Integration
{
    public class ServerApiRequest
    {
        public const string HTTP_METHOD_GET = "GET";
        public const string HTTP_METHOD_POST = "POST";
        public const string HTTP_METHOD_DELETE = "DELETE";
        public const string HTTP_METHOD_HEAD = "HEAD";
        

        private HttpWebRequest request;
        private ServerApiRequest(HttpWebRequest request)
        {
            this.request = request;
        }

        public void Send(Action<string> success, Action<WebException, string> error, int timeout = 0)
          {
            try
            {
                if (timeout > 0)
                    request.Timeout = timeout * 1000;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                         StreamReader reader = new StreamReader(response.GetResponseStream()); 
                            success(reader.ReadToEnd());
                    }
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    error(ex, reader.ReadToEnd());
                }
            }
        }

        public static ServerApiRequest Create(string url, string method = HTTP_METHOD_GET, string data = null, Dictionary<string, string> headers = null)
        {
            //specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = 1 * 60 * 1000; // 1 min
            request.Method = method;

            if(headers != null)
            {
                foreach(var h in headers)
                {
                    request.Headers.Add(h.Key, h.Value);
                }
            }

            if (data != null)
            {
                request.ContentType = "application/json; charset=utf-8";

                byte[] bodyData = UTF8Encoding.UTF8.GetBytes(data);
                request.ContentLength = bodyData.Length;
                Stream postStream = request.GetRequestStream();
                postStream.Write(bodyData, 0, bodyData.Length);
            }

            return new ServerApiRequest(request);
        }
    }
}
