using WebWindows.Blazor;
using System;
using System.IO;
using System.Runtime.InteropServices;

//테스트
namespace OpenNetLinkApp
{
    public class Program
    {

        [DllImport("user32.dll")]
        public static extern int PostMessage(int hwnd, uint wMsg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32")]
        static extern bool IsWindow(int hWnd);


        static void Main(string[] args)
        {
            string cwdPath = "";
            string CWD = Directory.GetCurrentDirectory();
            Console.WriteLine("==> {0}", CWD);

            bool flagMutex;
            System.Threading.Mutex m_hMutex = new System.Threading.Mutex(true, "OpenNetLink", out flagMutex);

//#if DEBUG
#if (DEBUG || !DEBUG)
            if (true)
#else
            if (flagMutex)
#endif
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
#if DEBUG
                        cwdPath = Environment.CurrentDirectory;
#else
                        cwdPath = "/Applications/OpenNetLinkApp.app/Contents/MacOS";
#endif
                }
                else 
                {
                    cwdPath = Environment.CurrentDirectory;
                }

                Directory.SetCurrentDirectory(cwdPath);
                ComponentsDesktop.Run<Startup>("OpenNetLink", "wwwroot/index.html");
            }
            else
            {
                int nhWnd = 0;
                nhWnd = FindWindow("WebWindow", "OpenNetLink");
                if (nhWnd != 0 && IsWindow(nhWnd))
                {
                    PostMessage(nhWnd, 0x0400+0x0003, 0, 0);
                }

            }

        }
    }
}
