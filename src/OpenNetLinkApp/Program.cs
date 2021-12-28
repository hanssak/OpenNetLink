using WebWindows.Blazor;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OpenNetLinkApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string cwdPath = "";
            string CWD = Directory.GetCurrentDirectory();
            Console.WriteLine("==> {0}", CWD);

            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                cwdPath = "/Applications/OpenNetLinkApp.app/Contents/MacOS";
            }
            else 
            {
                cwdPath = Environment.CurrentDirectory;
            }

            Directory.SetCurrentDirectory(cwdPath);
            ComponentsDesktop.Run<Startup>("OpenNetLink", "wwwroot/index.html");
        }
    }
}
