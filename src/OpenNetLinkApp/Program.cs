using WebWindows.Blazor;
using System;
using System.IO;

namespace OpenNetLinkApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string cmdPath = "/Applications/OpenNetLinkApp.app/Contents/MacOS";

            Directory.SetCurrentDirectory(cmdPath);
            ComponentsDesktop.Run<Startup>("OpenNetLink", "wwwroot/index.html");
        }
    }
}
