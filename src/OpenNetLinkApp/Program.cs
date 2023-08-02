using WebWindows.Blazor;
using System;
using System.IO;
using System.Runtime.InteropServices;
using HsNetWorkSG;

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

#if DEBUG
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

                //HsNetWorkSG.SGCrypto.UseKeyGen = false;
                //OP 파일 재 암호화 => 암복호화 실패 시 재설치
                //Network 파일 재 암호화 => 암복호화 실패 시 재설치
                bool kekGenInit = (!File.Exists("wwwroot/conf/hsck"));
                SGCrypto.ValidationResult = true;

                if (kekGenInit)
                    SGCrypto.ValidationResult &= SGCrypto.SaveKeyGenerate("wwwroot/conf/hsck");
                else
                    SGCrypto.ValidationResult &= SGCrypto.LoadKeyGenerate("wwwroot/conf/hsck");

                //json파일 재암호화
                //파일에 저장된 항목 dek 재 암호화
                SGCrypto.ValidationResult &= SGFileCrypto.Instance.EncryptSettingFiles();

                object[] arg = new object[2];
                arg[0] = Services.SGAppManager.SGAppConfigService.AppConfigInfo.bStartProgramReg;
                arg[1] = Services.SGAppManager.SGAppConfigService.AppConfigInfo.bStartTrayMove;

                ComponentsDesktop.Run<Startup>("OpenNetLink", "wwwroot/index.html", arg);
            }
            else
            {
                int nhWnd = 0;
                nhWnd = FindWindow("WebWindow", "OpenNetLink");
                if (nhWnd != 0 && IsWindow(nhWnd))
                {
                    PostMessage(nhWnd, 0x0400 + 0x0003, 0, 0);
                }

            }

        }
    }
}
