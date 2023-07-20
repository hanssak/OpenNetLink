using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using WebWindows;

namespace PreviewUtil
{
    class Program
    {
        private static string strUrl = "http://localhost:9997/";
        static void Main(string[] args)
        {
            string url = "https://daum.net";//args[0];
            string utilType = "OKTA";//(args.Length > 1) ? args[1] : "   PreviewUtilApp";
            utilType = utilType.ToUpper();

            string title = "   PreviewUtilApp";
            if (utilType == "OKTA")
                title = "   OKTAAuthentication";

            var window = new WebWindow(title, options =>
            {
                options.SchemeHandlers.Add("app", (string url, out string contentType) =>
                {
                    contentType = "text/javascript";
                    return new MemoryStream(Encoding.UTF8.GetBytes(""));
                });
            });

            window.OnWebMessageReceived += (sender, message) =>
            {
                window.SendMessage("Got message: " + message);
            };

            // Console.WriteLine("0: {0}", args[0]);
            window.NavigateToUrl("https://daum.net");
            window.SetTrayStartUse(false);

            switch (utilType)
            {
                case "OKTA":
                    window.URLChanged += (sender, url) =>
                    {
                        //window.SendMessage("OKTA Util (Changed URL): " + url);
                        //WaitForURLRedirection(url);
                        //ChangedURL(window, sender, url);
                        if(url.Contains("daum"))
                            Console.WriteLine("TESTTESTTEST");

                        Console.WriteLine($"!!!!!!!!!!!!!!!!!!!!!11");    
                        Console.WriteLine($"URL - {url}");
                    };
                    break;
                default:        //case "PreviewUtilApp":
                    break;
            }

            window.WaitForExit();

        }

        static void ChangedURL(WebWindow window, object sender, string URL)
        {


            Console.WriteLine(URL);
        }

        ///// <summary>
        ///// URL �����̷��� �̺�Ʈ
        ///// </summary>
        //static void _WaitForURLRedirection()
        //{
        //    // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        //    HttpClient client = new HttpClient();
        //    try
        //    {
        //        // Call asynchronous network methods in a try/catch block to handle exceptions.
        //        try
        //        {
        //            using HttpResponseMessage response = await client.Send("http://www.contoso.com/");
        //            response.EnsureSuccessStatusCode();
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            // Above three lines can be replaced with new helper method below
        //            // string responseBody = await client.GetStringAsync(uri);

        //            Console.WriteLine(responseBody);
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            Console.WriteLine("\nException Caught!");
        //            Console.WriteLine("Message :{0} ", e.Message);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        static void WaitForURLRedirection(string oktaResultURL, int ReqTimeOut = 2)
        {
            string listenrURL = strUrl;
            try
            {
                WebRequest wreq = WebRequest.Create(listenrURL);
                //ResendResponse? resp = new ResendResponse();
                wreq.Method = "POST";
                wreq.Timeout = ReqTimeOut * 1000;
                wreq.ContentType = "application/text; utf-8";

                using (var streamWriter = new StreamWriter(wreq.GetRequestStream())) //����
                {
                    streamWriter.Write(oktaResultURL);
                }
                Console.WriteLine("OKTAUtil => OpenNetLink SendURL");

                var response = wreq.GetResponse();
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err.ToString());
            }

        }
    }
}
