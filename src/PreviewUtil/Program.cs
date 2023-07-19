using System;
using System.IO;
using System.Text;
using WebWindows;

namespace PreviewUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = args[0];
            string title = (args.Length > 1) ? args[1] : "   PreviewUtilApp";

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
            window.NavigateToUrl(args[0]);
            window.SetTrayStartUse(false);
            window.WaitForExit();
        }
    }
}
