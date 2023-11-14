using WebWindows.Blazor;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Serilog.Events;
using AgLogManager;
using Serilog;
using HsNetWorkSG;

//테스트
namespace OpenNetLinkApp
{
    public class Program
    {
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<Program>();

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

            Services.SGAppManager.SGSystemService.SystemInfo.StartArg = args;

            string windowTitle = Common.CsFunction.XmlConf.GetTitle("T_WINDOW_TITLE");
            if (String.IsNullOrEmpty(windowTitle))
                windowTitle = "OpenNetLink";

#if DEBUG
            if (true)
#else
            if (flagMutex)
#endif
            {
                InitializeLogger();

                if (args?.Length > 0) CLog.Information($"args : {string.Join(',', args)}");

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


                //SP1 테스트를 위해 처리
                //SGFileCrypto.Instance.EncryptSettingFiles("wwwroot/conf/SP1_DEBUG");

                object[] arg = new object[2];
                arg[0] = Services.SGAppManager.SGAppConfigService.AppConfigInfo.bStartProgramReg;
                arg[1] = Services.SGAppManager.SGAppConfigService.AppConfigInfo.bStartTrayMove;

                string hiddenFlagFile = Path.Combine("wwwroot/Log", "UseHiddenLog");
                if (File.Exists(hiddenFlagFile))
                {
                    HsNetWork.UseHiddenLog = true;
                    File.Delete(hiddenFlagFile);
                }

                ComponentsDesktop.Run<Startup>(windowTitle, "wwwroot/index.html", arg);
            }
            else if (Services.SGAppManager.SGopConfigService.AppConfigInfo[0].NACLoginType == (int)Common.Enums.enumNacLoginType.Genian && args?.Length > 0 && args[0].ToString().ToUpper() == "NAC")
            {
                //NAC으로 로그인 시
                //기존 OpenNetLink로 종료요청("REQUEST_OPENNETLINK_EXIT" 키워드 전달)
                HsNetWorkSG.HsContextSender.RequestExitSender();

                //정상종료 불가 시 강제종료
                Thread.Sleep(1000);
                Process[] OpenNetLinkes = Process.GetProcessesByName("OpenNetLinkApp");
                if (OpenNetLinkes.Length > 1)
                    OpenNetLinkes[0].Kill();

                InitializeLogger();

                if (args?.Length > 0) CLog.Information($"args : {string.Join(',', args)}");

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
                    PostMessage(nhWnd, 0x0400 + 0x0003, 0, 0);
                }
            }

        }

        /// <summary>
        /// 로그 기록 활성화
        /// </br>[위치이동] RunAsync보다 먼저 활성화하여 화면 로드 전에 로그 기록할 수 있도록 이동
        /// </summary>
        static void InitializeLogger()
        {
            try
            {
                /* Configuration Log */
                AgLog.LogLevelSwitch.MinimumLevel = LogEventLevel.Information;
                string strLogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}][APP:{ProcessName}][PID:{ProcessId}][THR:{ThreadId}]{operationId} {Message} ";
                strLogTemplate += "{MemberName} {FilePath}{LineNumber}{NewLine}{Exception}";

                string Path = System.IO.Path.Combine(System.Environment.CurrentDirectory, "wwwroot");
                Path = System.IO.Path.Combine(Path, "Log");
                System.IO.Directory.CreateDirectory(Path);
                Path = System.IO.Path.Combine(Path, "SecureGate-{Date}.Log");
                Serilog.Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .Enrich.WithProcessName()
                                .Enrich.WithProcessId()
                                .Enrich.WithThreadId()
                                .Enrich.With<OperationIdEnricher>()
                                .WriteTo.RollingFile(Path,
                                                    //rollingInterval: RollingInterval.Day, 
                                                    //rollOnFileSizeLimit: true,
                                                    fileSizeLimitBytes: 1024 * 1024 * 100,
                                                    retainedFileCountLimit: 31,
                                                    buffered: false,
                                                    outputTemplate: strLogTemplate)
                                // .WriteTo.Console(outputTemplate: strLogTemplate, theme: AnsiConsoleTheme.Literate)
                                .MinimumLevel.ControlledBy(AgLog.LogLevelSwitch)
                                .CreateLogger();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitializeLogger Exception : {ex.ToString()}");
                throw;
            }
        }
    }
}
