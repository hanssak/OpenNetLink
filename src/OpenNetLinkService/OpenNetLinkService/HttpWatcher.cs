
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using OpenNetLinkService.Site;

namespace OpenNetLinkService
{
    class HttpWatcher
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<HttpWatcher>();
        public static void Run()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenNetLinkService.json");
                string configJson = File.ReadAllText(configPath);
                Config config = Config.FromJsonText(configJson);

                HttpListener listener = new HttpListener();

                listener.Prefixes.Add($"http://{config.Ip}:{config.port}/");

                listener.Start();

                while (true)
                {
                    HttpListenerContext ctx = listener.GetContext();
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;

                    if (config.SiteName == "KoReg")
                    {
                        int result = KoReg.ProcessPerform(req);
                    }

                    resp.StatusCode = (int)HttpStatusCode.OK;       // 200
                    resp.StatusDescription = "OK";

                    resp.Close();
                }
            }
            catch (Exception ex)
            {
                CLog.Error($"OpenNetLink Service HttpWatcher Exception - {ex.Message}");
            }
        }
    }
}
