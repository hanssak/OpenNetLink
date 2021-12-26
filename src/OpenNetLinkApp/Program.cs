using WebWindows.Blazor;
using System;
using System.IO;

namespace OpenNetLinkApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string CWD = Directory.GetCurrentDirectory();
            Console.WriteLine("==> {0}", CWD);
            Directory.SetCurrentDirectory("/Applications/OpenNetLinkApp.app/Contents/MacOS");
            ComponentsDesktop.Run<Startup>("OpenNetLink", "wwwroot/index.html");
        }
    }
}
