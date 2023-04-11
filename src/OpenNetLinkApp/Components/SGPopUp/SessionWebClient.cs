using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AgLogManager;
using Serilog;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Components.SGPopUp
{
    class SessionWebClient
    {

        private static Serilog.ILogger CLog => Serilog.Log.ForContext<SessionWebClient>();


        private WebClient m_webClient = new WebClient();
        private CookieContainer m_cookieContainer = new CookieContainer();
        private string m_Cookie = "";
        public SessionWebClient()
        {
            m_webClient.Proxy = null;
            m_webClient.Encoding = System.Text.Encoding.UTF8;
        }
        //세션및 SSL 사용시 문제 발생해 Request를 새로 만들어 사용함        
        public String PostRequest(String url, String postData)
        {
            String responseJSON = String.Empty;
            try
            {
                m_webClient.Headers["Cookie"] = m_Cookie;
                m_webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                responseJSON = m_webClient.UploadString(url, postData);
                if (m_webClient.ResponseHeaders["Set-Cookie"] != null)
                    m_Cookie = m_webClient.ResponseHeaders["Set-Cookie"];
                return responseJSON;
            }
            catch (Exception ex)
            {
                CLog.Here().Error(string.Format("PostRequest Exception - {0}", ex.Message));
            }
            return responseJSON;
        }

        public String Request(String url, String postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = null;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.CookieContainer = m_cookieContainer;

            TrustAllCert ValCallback = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValCallback.OnValidationCallback);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            string responseJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseJSON;
        }

        /// <summary>
        /// HTTPS(SSL-Request) 동작함
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="strContentType"></param>
        /// <returns></returns>
        public String RequestUsePostJson(String url, String postData, string strContentType)
        {

            string responseJSON = "";

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = null;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = strContentType; // "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.CookieContainer = m_cookieContainer;

                TrustAllCert ValCallback = new TrustAllCert();
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValCallback.OnValidationCallback);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                responseJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();

            }
            catch(Exception ex)
            {
                Log.Error(string.Format($"RequestUsePostJson Exception - Msg : {ex.Message}"));
            }

            return responseJSON;
        }

        public async Task<string> RequestUsePostJsonOtherType(String url, String postData, string strContentType)
        {

            string responseJSON = "";

            try
            {
                // Serialize the data to JSON

                using (var httpClient = new HttpClient())
                {
                    // Serialize the data to JSON
                    //var jsonData = JsonConvert.SerializeObject(postData);

                    // Create a StringContent object to hold the serialized JSON data
                    var content = new StringContent(postData, System.Text.Encoding.UTF8, "application/json");

                    // Send the POST request to the endpoint
                    var response = await httpClient.PostAsync(url, content);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        responseJSON = await response.Content.ReadAsStringAsync();
                        Log.Logger.Here().Information($"RequestUsePostJsonOtherType, Read the response content!");
                        //Console.WriteLine("Data sent successfully.");
                    }
                    else
                    {
                        Log.Logger.Here().Information($"RequestUsePostJsonOtherType, Read the response Error : {response.StatusCode} - {response.ReasonPhrase}");
                        //Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }

                /*var request = (HttpWebRequest)WebRequest.Create(url);
               request.Proxy = null;
               var data = Encoding.ASCII.GetBytes(postData);

               request.Method = "POST";
               request.ContentType = strContentType; // "application/x-www-form-urlencoded";
               request.ContentLength = data.Length;
               request.CookieContainer = m_cookieContainer;

               TrustAllCert ValCallback = new TrustAllCert();
               ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValCallback.OnValidationCallback);

               using (var stream = request.GetRequestStream())
               {
                   stream.Write(data, 0, data.Length);
               }
               var response = (HttpWebResponse)request.GetResponse();
               responseJSON = new StreamReader(response.GetResponseStream()).ReadToEnd();*/

            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error(string.Format($"RequestUsePostJson Exception - Msg : {ex.Message}"));
            }

            return responseJSON;
        }


        

    }

    public class TrustAllCert
    {
        public TrustAllCert()
        {
        }
        public bool OnValidationCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}

