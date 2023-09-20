using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using WebWindows;

namespace PreviewUtil
{
    class Program
    {
        enum UtilType
        {
            PreviewUtil,
            OKTA,
        }
        private static string strWatcherUrl = "http://localhost:9997/";
        static void Main(string[] args)
        {
            string url = args[0];
            UtilType utilType = UtilType.PreviewUtil;
            if (args.Length > 1 && Enum.TryParse(typeof(UtilType), args[1], out object parObj) == true)
                utilType = (UtilType)parObj;

            string title = string.Empty;
            if (utilType == UtilType.OKTA)
            {
                title = "   OKTA Authentication";
                strWatcherUrl = (args.Length > 2) ? args[2] : strWatcherUrl;
            }
            else //PreviewUtil
            {
                title = "   PreviewUtilApp";
            }

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
            window.SetUseHttpUrl(true);
            window.NavigateToUrl(url);
            window.SetTrayStartUse(false);

            if (utilType == UtilType.OKTA)
            {
                window.URLChanged += (sender, list) =>
                {
                    IntPtr bUri = (IntPtr)list[0];
                    int length = (int)list[1];
                    byte[] data = new byte[length];

                    Marshal.Copy(bUri, data, 0, length);

                    string urlDetectedValue = Encoding.UTF8.GetString(data);

                    WaitForOKTAURLDetect(urlDetectedValue);
                };
            }
            window.WaitForExit();
        }

        static void WaitForOKTAURLDetect(string oktaResultURL, int ReqTimeOut = 2)
        {
            string listenrURL = strWatcherUrl;
            WebRequest wreq;
            try
            {
                wreq = WebRequest.Create(listenrURL);
                wreq.Method = "POST";
                wreq.Timeout = ReqTimeOut * 1000;
                wreq.ContentType = "application/text; utf-8";

                using (var streamWriter = new StreamWriter(wreq.GetRequestStream())) //전송
                {
                    streamWriter.Write(oktaResultURL);
                }
                var response = wreq.GetResponse();
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }
    }
}
