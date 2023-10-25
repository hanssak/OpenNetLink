using WebWindows.Blazor;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

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
                //cwdPath = Environment.CurrentDirectory;
                //관리자가 사용자를 지정하여 OpenNetLink 실행 시에도 정상적인 경로로 표시하기 위해 변경
                cwdPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            }

            Directory.SetCurrentDirectory(cwdPath);

            string windowTitle = Common.CsFunction.XmlConf.GetTitle("T_WINDOW_TITLE");
            if (String.IsNullOrEmpty(windowTitle))
                windowTitle = "OpenNetLink";

#if DEBUG
            if (true)
#else
            if (flagMutex)
#endif
            {
                object[] arg = new object[2];
                arg[0] = Services.SGAppManager.SGAppConfigService.AppConfigInfo.bStartProgramReg;
                arg[1] = Services.SGAppManager.SGAppConfigService.AppConfigInfo.bStartTrayMove;

                ComponentsDesktop.Run<Startup>(windowTitle, "wwwroot/index.html", arg);
            }
            else
            {
                int nhWnd = 0;
                nhWnd = FindWindow("WebWindow", windowTitle);
                if (nhWnd != 0 && IsWindow(nhWnd))
                {
                    PostMessage(nhWnd, 0x0400+0x0003, 0, 0);
                }
            }
        }
    }
}
