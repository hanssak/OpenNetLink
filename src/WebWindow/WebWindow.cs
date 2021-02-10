using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Serilog;
using Serilog.Events;
using AgLogManager;
using WinClipLib;

namespace WebWindows
{
    [StructLayout(LayoutKind.Sequential)]
    struct NativeRect
    {
        public int x, y;
        public int width, height;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct NativeMonitor
    {
        public NativeRect monitor;
        public NativeRect work;
    }

    public readonly struct Monitor
    {
        public readonly Rectangle MonitorArea;
        public readonly Rectangle WorkArea;

        public Monitor(Rectangle monitor, Rectangle work)
        {
            MonitorArea = monitor;
            WorkArea = work;
        }

        internal Monitor(NativeRect monitor, NativeRect work)
            : this(new Rectangle(monitor.x, monitor.y, monitor.width, monitor.height), new Rectangle(work.x, work.y, work.width, work.height))
        { }

        internal Monitor(NativeMonitor nativeMonitor)
            : this(nativeMonitor.monitor, nativeMonitor.work)
        { }
    }

    /// <summary>
    /// 1) 온라인
    /// 2) 오프라인
    /// 3) 승인대기
    /// 4) 수신완료
    /// 5) 클립보드
    /// 6) 파일 수신대기
    /// 7) 정보보안결재
    /// 8) 이메일 결재
    /// 9) 승인반려 알림
    /// 10) 승인완료 알림
    /// 11) 파일서버 검사 알림
    /// 12) 공지사항
    /// 13) URL 신청 승인완료 알림
    /// 14) URL 신청 반려 알림
    /// 15) URL 등록 완료 알림
    /// 16) URL 등록 취소 알림.
    /// 17) URL 결재 대기 알림
    /// 18) 서버 바이러스 검사 알림  - 현재 사용 안함 ( 서버 바이러스 검사는 팝업창 사용)
    /// </summary>
    public enum OS_NOTI : int
    {
        ONLINE     = 1,
        OFFLINE,
        WAIT_APPR,
        RECV_DONE,
        CLIPBOARD,
        WAIT_FILE,
        SECURE_APPR,
        MAIL_APPR,
        REJECT_APPR,
        CONFIRM_APPR,
        CHECK_FILE,
        NOTICE,
        URL_CONFIRM_APPR,
        URL_REJECT_APPR,
        URL_REGI_CONFIRM,
        URL_REGI_CANCEL,
        URL_WAIT_APPR,
        CHECK_VIRUS,
    }

    public enum CLIPTYPE : int
    {
        TEXT = 1,
        IMAGE = 2,
        OBJECT = 3,
    }

    public struct ClipBoardData // readonly 
    {
        public int nGroupId;
        public readonly CLIPTYPE nType;
        public readonly int nLength;
        public readonly IntPtr pMem;
        public ClipBoardData(int nGroupId, CLIPTYPE nType, int nLength, IntPtr pMem)
        {
            this.nGroupId = nGroupId;
            this.nType = nType;
            this.nLength = nLength;
            this.pMem = pMem;
        }
    }


    public class WebWindow
    {
        // Here we use auto charset instead of forcing UTF-8.
        // Thus the native code for Windows will be much more simple.
        // Auto charset is UTF-16 on Windows and UTF-8 on Unix(.NET Core 3.0 and later and Mono).
        // As we target .NET Standard 2.1, we assume it runs on .NET Core 3.0 and later.
        // We should specify using auto charset because the default value is ANSI.

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)] delegate void OnWebMessageReceivedCallback(string message);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)] delegate IntPtr OnWebResourceRequestedCallback(string url, out int numBytes, out string contentType);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void InvokeCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int GetAllMonitorsCallback(in NativeMonitor monitor);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void ResizedCallback(int width, int height);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void MovedCallback(int x, int y);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void NTLogCallback(int nLevel, string message);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void ClipBoardCallback(int nGroupId, int nType, int nLength, IntPtr pMem);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void RecvClipBoardCallback(int nGroupId);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void RequestedNavigateURLCallback([MarshalAs(UnmanagedType.LPWStr)] string navURI);

        const string DllName = "WebWindow.Native";
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr WebWindow_register_win32(IntPtr hInstance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr WebWindow_register_mac();
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern IntPtr WebWindow_ctor(string title, IntPtr parentWebWindow, OnWebMessageReceivedCallback webMessageReceivedCallback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_dtor(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern IntPtr WebWindow_getHwnd_win32(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_SetTitle(IntPtr instance, string title);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_Show(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_WaitForExit(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_Invoke(IntPtr instance, InvokeCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_NavigateToString(IntPtr instance, string content);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_NavigateToUrl(IntPtr instance, string url);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_ShowMessage(IntPtr instance, string title, string body, uint type);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_SendMessage(IntPtr instance, string message);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_ShowUserNotification(IntPtr instance, string image, string title, string message, string navURI = null);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_AddCustomScheme(IntPtr instance, string scheme, OnWebResourceRequestedCallback requestHandler);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetResizable(IntPtr instance, int resizable);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_GetSize(IntPtr instance, out int width, out int height);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetSize(IntPtr instance, int width, int height);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetResizedCallback(IntPtr instance, ResizedCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_GetAllMonitors(IntPtr instance, GetAllMonitorsCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern uint WebWindow_GetScreenDpi(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_GetPosition(IntPtr instance, out int x, out int y);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetPosition(IntPtr instance, int x, int y);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetMovedCallback(IntPtr instance, MovedCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetTopmost(IntPtr instance, int topmost);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_SetIconFile(IntPtr instance, string filename);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetNTLogCallback(IntPtr instance, NTLogCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetClipBoardCallback(IntPtr instance, ClipBoardCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetRecvClipBoardCallback(IntPtr instance, RecvClipBoardCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)] static extern void WebWindow_SetRequestedNavigateURLCallback(IntPtr instance, RequestedNavigateURLCallback callback);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_RegClipboardHotKey(IntPtr instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_UnRegClipboardHotKey(IntPtr instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_RegClipboardHotKeyNetOver(IntPtr instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_UnRegClipboardHotKeyNetOver(IntPtr instance, int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_FolderOpen(IntPtr instance, string strFileDownPath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_OnHotKey(IntPtr instance, int groupID);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_SetClipBoardData(IntPtr instance, int nGroupID, int nType, int nClipSize, byte[] data);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_ProgramExit(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_SetTrayUse(IntPtr instance, bool useTray);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_MoveWebWindowToTray(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_RegStartProgram(IntPtr instance);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)] static extern void WebWindow_UnRegStartProgram(IntPtr instance);

        private readonly List<GCHandle> _gcHandlesToFree = new List<GCHandle>();
        private readonly List<IntPtr> _hGlobalToFree = new List<IntPtr>();
        private readonly IntPtr _nativeWebWindow;
        private readonly int _ownerThreadId;
        private string _title;
        private static Serilog.ILogger CLog => Serilog.Log.ForContext<WebWindow>();

        public WinClipboardLibray winClip = null;
        static WebWindow()
        {
            // Workaround for a crashing issue on Linux. Without this, applications
            // are crashing when running in Debug mode (but not Release) if the very
            // first line of code in Program::Main references the WebWindow type.
            // It's unclear why.
            Thread.Sleep(1);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hInstance = Marshal.GetHINSTANCE(typeof(WebWindow).Module);
                WebWindow_register_win32(hInstance);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                WebWindow_register_mac();
            }
        }

        public WebWindow(string title) : this(title, _ => { })
        {
        }

        public WebWindow(string title, Action<WebWindowOptions> configure)
        {
            _ownerThreadId = Thread.CurrentThread.ManagedThreadId;

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new WebWindowOptions();
            configure.Invoke(options);

            WriteTitleField(title);

            var onWebMessageReceivedDelegate = (OnWebMessageReceivedCallback)ReceiveWebMessage;
            _gcHandlesToFree.Add(GCHandle.Alloc(onWebMessageReceivedDelegate));

            var parentPtr = options.Parent?._nativeWebWindow ?? default;
            _nativeWebWindow = WebWindow_ctor(_title, parentPtr, onWebMessageReceivedDelegate);

            foreach (var (schemeName, handler) in options.SchemeHandlers)
            {
                AddCustomScheme(schemeName, handler);
            }

            var onResizedDelegate = (ResizedCallback)OnResized;
            _gcHandlesToFree.Add(GCHandle.Alloc(onResizedDelegate));
            WebWindow_SetResizedCallback(_nativeWebWindow, onResizedDelegate);

            var onMovedDelegate = (MovedCallback)OnMoved;
            _gcHandlesToFree.Add(GCHandle.Alloc(onMovedDelegate));
            WebWindow_SetMovedCallback(_nativeWebWindow, onMovedDelegate);

            var onNTLogDelegate = (NTLogCallback)OnNTLog;
            _gcHandlesToFree.Add(GCHandle.Alloc(onNTLogDelegate));
            WebWindow_SetNTLogCallback(_nativeWebWindow, onNTLogDelegate);

            var onClipBoardDelegate = (ClipBoardCallback)OnClipBoard;
            _gcHandlesToFree.Add(GCHandle.Alloc(onClipBoardDelegate));
            WebWindow_SetClipBoardCallback(_nativeWebWindow, onClipBoardDelegate);

            var onRecvClipBoardDelegate = (RecvClipBoardCallback)OnRecvClipBoard;
            _gcHandlesToFree.Add(GCHandle.Alloc(onRecvClipBoardDelegate));
            WebWindow_SetRecvClipBoardCallback(_nativeWebWindow, onRecvClipBoardDelegate);

            var onRequestedNavigateURLDelegate = (RequestedNavigateURLCallback)OnRequestedNavigateURL;
            _gcHandlesToFree.Add(GCHandle.Alloc(onRequestedNavigateURLDelegate));
            WebWindow_SetRequestedNavigateURLCallback(_nativeWebWindow, onRequestedNavigateURLDelegate);

            // Auto-show to simplify the API, but more importantly because you can't
            // do things like navigate until it has been shown
            Show();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                winClip = new WinClipboardLibray();
                winClip.SetRecvHotKeyEvent(WinOnHotKey);
                //winClip.RegHotKey(0, false, true, true, false, 'V');
            }
        }

        ~WebWindow()
        {
            // TODO: IDisposable
            WebWindow_SetResizedCallback(_nativeWebWindow, null);
            WebWindow_SetMovedCallback(_nativeWebWindow, null);
            WebWindow_SetNTLogCallback(_nativeWebWindow, null);
            WebWindow_SetClipBoardCallback(_nativeWebWindow, null);
            WebWindow_SetRecvClipBoardCallback(_nativeWebWindow, null);
            WebWindow_SetRequestedNavigateURLCallback(_nativeWebWindow, null);
            foreach (var gcHandle in _gcHandlesToFree)
            {
                gcHandle.Free();
            }
            _gcHandlesToFree.Clear();
            foreach (var handle in _hGlobalToFree)
            {
                Marshal.FreeHGlobal(handle);
            }
            _hGlobalToFree.Clear();
            WebWindow_dtor(_nativeWebWindow);
        }

        public void WinOnHotKey(int groupID) => WebWindow_OnHotKey(_nativeWebWindow, groupID);

        public void Show() => WebWindow_Show(_nativeWebWindow);
        public void WaitForExit() => WebWindow_WaitForExit(_nativeWebWindow);

        public string Title
        {
            get => _title;
            set
            {
                WriteTitleField(value);
                WebWindow_SetTitle(_nativeWebWindow, _title);
            }
        }

        public void ShowMessage(string title, string body)
        {
            WebWindow_ShowMessage(_nativeWebWindow, title, body, /* MB_OK */ 0);
        }

        public void Invoke(Action workItem)
        {
            // If we're already on the UI thread, no need to dispatch
            if (Thread.CurrentThread.ManagedThreadId == _ownerThreadId)
            {
                workItem();
            }
            else
            {
                WebWindow_Invoke(_nativeWebWindow, workItem.Invoke);
            }
        }

        public IntPtr Hwnd
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return WebWindow_getHwnd_win32(_nativeWebWindow);
                }
                else
                {
                    throw new PlatformNotSupportedException($"{nameof(Hwnd)} is only supported on Windows");
                }
            }
        }

        public void NavigateToString(string content)
        {
            WebWindow_NavigateToString(_nativeWebWindow, content);
        }

        public void NavigateToUrl(string url)
        {
            WebWindow_NavigateToUrl(_nativeWebWindow, url);
        }

        public void NavigateToLocalFile(string path)
        {
            var absolutePath = Path.GetFullPath(path);
            var url = new Uri(absolutePath, UriKind.Absolute);
            NavigateToUrl(url.ToString());
        }

        public void SendMessage(string message)
        {
            WebWindow_SendMessage(_nativeWebWindow, message);
        }

        public void ShowUserNotification(string image, string title, string message, string navURI = null)
        {
            //WebWindow_ShowUserNotification(_nativeWebWindow, image, title, message, navURI);
            Invoke(() => WebWindow_ShowUserNotification(_nativeWebWindow,image, title, message, navURI));   // KKW
        }

        public void Notification(OS_NOTI category, string title, string message, string navURI = "")
        {
            string image = String.Format($"wwwroot/images/noti/{(int)category}.png");
            Log.Information("ImageString: " + image);

            /*
            switch(category)
            {
                case OS_NOTI.ONLINE              : { image = "wwwroot/images/noti/1.png";  } break;
                case OS_NOTI.OFFLINE             : { image = "wwwroot/images/noti/2.png";  } break;
                case OS_NOTI.WAIT_APPR           : { image = "wwwroot/images/noti/3.png";  } break;
                case OS_NOTI.RECV_DONE           : { image = "wwwroot/images/noti/4.png";  } break;
                case OS_NOTI.CLIPBOARD           : { image = "wwwroot/images/noti/5.png";  } break;
                case OS_NOTI.WAIT_FILE           : { image = "wwwroot/images/noti/6.png";  } break;
                case OS_NOTI.SECURE_APPR         : { image = "wwwroot/images/noti/7.png";  } break;
                case OS_NOTI.MAIL_APPR           : { image = "wwwroot/images/noti/8.png";  } break;
                case OS_NOTI.REJECT_APPR         : { image = "wwwroot/images/noti/9.png";  } break;
                case OS_NOTI.CONFIRM_APPR        : { image = "wwwroot/images/noti/10.png"; } break;
                case OS_NOTI.CHECK_FILE          : { image = "wwwroot/images/noti/11.png"; } break;
                case OS_NOTI.NOTICE              : { image = "wwwroot/images/noti/12.png"; } break;
                case OS_NOTI.URL_CONFIRM_APPR    : { image = "wwwroot/images/noti/13.png"; } break;
                case OS_NOTI.URL_REJECT_APPR     : { image = "wwwroot/images/noti/14.png"; } break;
                case OS_NOTI.URL_REGI_CONFIRM    : { image = "wwwroot/images/noti/15.png"; } break;
                case OS_NOTI.URL_REGI_CANCEL     : { image = "wwwroot/images/noti/16.png"; } break;
                case OS_NOTI.URL_WAIT_APPR       : { image = "wwwroot/images/noti/17.png"; } break;
                case OS_NOTI.CHECK_VIRUS         : { image = "wwwroot/images/noti/18.png"; } break;
            }
            */
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                image = Path.Combine(System.IO.Directory.GetCurrentDirectory(), image);
                image = image.Replace("/", "\\");
            }
            ShowUserNotification(image, title, message, navURI);
        }

        public event EventHandler<string> OnWebMessageReceived;

        private void WriteTitleField(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "Untitled window";
            }

            // Due to Linux/Gtk platform limitations, the window title has to be no more than 31 chars
            if (value.Length > 31 && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                value = value.Substring(0, 31);
            }

            _title = value;
        }

        private void ReceiveWebMessage(string message)
        {
            OnWebMessageReceived?.Invoke(this, message);
        }

        private void AddCustomScheme(string scheme, ResolveWebResourceDelegate requestHandler)
        {
            // Because of WKWebView limitations, this can only be called during the constructor
            // before the first call to Show. To enforce this, it's private and is only called
            // in response to the constructor options.
            OnWebResourceRequestedCallback callback = (string url, out int numBytes, out string contentType) =>
            {
                var responseStream = requestHandler(url, out contentType);
                if (responseStream == null)
                {
                    // Webview should pass through request to normal handlers (e.g., network)
                    // or handle as 404 otherwise
                    numBytes = 0;
                    return default;
                }

                // Read the stream into memory and serve the bytes
                // In the future, it would be possible to pass the stream through into C++
                using (responseStream)
                using (var ms = new MemoryStream())
                {
                    responseStream.CopyTo(ms);

                    numBytes = (int)ms.Position;
                    var buffer = Marshal.AllocHGlobal(numBytes);
                    Marshal.Copy(ms.GetBuffer(), 0, buffer, numBytes);
                    _hGlobalToFree.Add(buffer);
                    return buffer;
                }
            };

            _gcHandlesToFree.Add(GCHandle.Alloc(callback));
            WebWindow_AddCustomScheme(_nativeWebWindow, scheme, callback);
        }

        private bool _resizable = true;
        public bool Resizable
        {
            get => _resizable;
            set
            {
                if (_resizable != value)
                {
                    _resizable = value;
                    Invoke(() => WebWindow_SetResizable(_nativeWebWindow, _resizable ? 1 : 0));
                }
            }
        }

        private int _width;
        private int _height;

        private void GetSize() => WebWindow_GetSize(_nativeWebWindow, out _width, out _height);

        private void SetSize() => Invoke(() => WebWindow_SetSize(_nativeWebWindow, _width, _height));

        public int Width
        {
            get
            {
                GetSize();
                return _width;
            }
            set
            {
                GetSize();
                if (_width != value)
                {
                    _width = value;
                    SetSize();
                }
            }
        }

        public int Height
        {
            get
            {
                GetSize();
                return _height;
            }
            set
            {
                GetSize();
                if (_height != value)
                {
                    _height = value;
                    SetSize();
                }
            }
        }

        public Size Size
        {
            get
            {
                GetSize();
                return new Size(_width, _height);
            }
            set
            {
                if (_width != value.Width || _height != value.Height)
                {
                    _width = value.Width;
                    _height = value.Height;
                    SetSize();
                    CLog.Here().Information("Window Size Is Setted {width} {height}", _width, _height);
                    Log.Information("Window Size Is Setted {width} {height}", _width, _height);
                }
            }
        }

        private void OnResized(int width, int height) => SizeChanged?.Invoke(this, new Size(width, height));

        public event EventHandler<Size> SizeChanged;

        private int _x;
        private int _y;

        private void GetPosition() => WebWindow_GetPosition(_nativeWebWindow, out _x, out _y);

        private void SetPosition() => Invoke(() => WebWindow_SetPosition(_nativeWebWindow, _x, _y));

        public int Left
        {
            get
            {
                GetPosition();
                return _x;
            }
            set
            {
                GetPosition();
                if (_x != value)
                {
                    _x = value;
                    SetPosition();
                }
            }
        }

        public int Top
        {
            get
            {
                GetPosition();
                return _y;
            }
            set
            {
                GetPosition();
                if (_y != value)
                {
                    _y = value;
                    SetPosition();
                }
            }
        }

        public Point Location
        {
            get
            {
                GetPosition();
                return new Point(_x, _y);
            }
            set
            {
                if (_x != value.X || _y != value.Y)
                {
                    _x = value.X;
                    _y = value.Y;
                    SetPosition();
                }
            }
        }

        private void OnMoved(int x, int y) => LocationChanged?.Invoke(this, new Point(x, y));

        public event EventHandler<Point> LocationChanged;

        public IReadOnlyList<Monitor> Monitors
        {
            get
            {
                List<Monitor> monitors = new List<Monitor>();
                int callback(in NativeMonitor monitor)
                {
                    monitors.Add(new Monitor(monitor));
                    return 1;
                }
                WebWindow_GetAllMonitors(_nativeWebWindow, callback);
                return monitors;
            }
        }

        public uint ScreenDpi => WebWindow_GetScreenDpi(_nativeWebWindow);

        private bool _topmost = false;
        public bool Topmost
        {
            get => _topmost;
            set
            {
                if (_topmost != value)
                {
                    _topmost = value;
                    Invoke(() => WebWindow_SetTopmost(_nativeWebWindow, _topmost ? 1 : 0));
                }
            }
        }

        public void SetIconFile(string filename) => WebWindow_SetIconFile(_nativeWebWindow, Path.GetFullPath(filename));
        private void OnNTLog(int nLevel, string message)
        {
            switch (nLevel)
            {
                case (int)LogEventLevel.Verbose: Log.Verbose(message); break;
                case (int)LogEventLevel.Debug: Log.Debug(message); break;
                case (int)LogEventLevel.Information: Log.Information(message); break;
                case (int)LogEventLevel.Warning: Log.Warning(message); break;
                case (int)LogEventLevel.Error: Log.Error(message); break;
                case (int)LogEventLevel.Fatal: Log.Fatal(message); break;
            }
        }
        // Classify by type and Send Clipboard
        private void OnClipBoard(int nGroupId, int nType, int nLength, IntPtr pMem) => ClipBoardOccured?.Invoke(this, new ClipBoardData(nGroupId, (CLIPTYPE)nType, nLength, pMem));
        public event EventHandler<ClipBoardData> ClipBoardOccured;

        private void OnRecvClipBoard(int nGroupId) => RecvClipBoardOccured?.Invoke(this, nGroupId);
        public event EventHandler<int> RecvClipBoardOccured;

        public void RegClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode) => WebWindow_RegClipboardHotKey(_nativeWebWindow, groupID, bAlt, bControl, bShift, bWin, chVKCode);
        public void UnRegClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode) => WebWindow_UnRegClipboardHotKey(_nativeWebWindow, groupID, bAlt, bControl, bShift, bWin, chVKCode);


        public void RegClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx) => WebWindow_RegClipboardHotKeyNetOver(_nativeWebWindow, groupID, bAlt, bControl, bShift, bWin, chVKCode, nIdx);

        public void UnRegClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx) => WebWindow_UnRegClipboardHotKeyNetOver(_nativeWebWindow, groupID, bAlt, bControl, bShift, bWin, chVKCode, nIdx);


        //public delegate void WinRegHotKeyEvent(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char ch);
        //public event EventHandler<ClipBoardData> ClipBoardOccured;
        //public delegate void WinUnRegHotKeyEvent(int groupID);
        public void WinRegClipboardHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
        {
            WinUnRegClipboardHotKey(groupID);
            Invoke(() => winClip.RegHotKey(groupID, bAlt, bControl, bShift, bWin, chVKCode));
        }
        public void WinUnRegClipboardHotKey(int groupID)
        {
            Invoke(() => winClip.UnRegHotKey(groupID));
        }

        public void WinRegClipboardHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
        {
            WinUnRegClipboardHotKeyNetOver(groupID, nIdx);
            Invoke(() => winClip.RegHotKeyNetOver(groupID, bAlt, bControl, bShift, bWin, chVKCode, nIdx));
        }

        public void WinUnRegClipboardHotKeyNetOver(int groupID, int nIdx)
        {
            Invoke(() => winClip.UnRegHotKeyNetOver(groupID, nIdx));
        }

        public void FolderOpen(string strFileDownPath) => WebWindow_FolderOpen(_nativeWebWindow,strFileDownPath);
        public void OpenFolder(string strFileDownPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                FolderOpen(strFileDownPath);
            }
            else
            {
                using (Process proc = new Process())
                {
                    strFileDownPath = strFileDownPath.Replace("\\","/");
                    try
                    {

                        // ps -ef | grep nemo | egrep -v 'grep|bash|sh|nemo-desktop'
                        string retMsg = RunExternalExe(filename: "ps", arguments: "-ef", useRedirectIO: true);
                        string[] arrayList = retMsg.Split("\n");
                        List<string> retList = arrayList.Where(item => item.Contains("nemo "))
                                                        .Select(item => item).ToList();
                        if(strFileDownPath != null) 
                        {
                            foreach( var line in retList)
                            {
                                if(line.Contains(strFileDownPath))
                                {
                                    foreach(var sval in line.Split(" "))
                                    {
                                        int ProcId;
                                        bool result = int.TryParse(sval, out ProcId);
                                        if(result)
                                        {
                                            CLog.Here().Information("Before Folder Oepn: previous nemo({0}) process is kill", ProcId);
                                            Process localById = Process.GetProcessById(ProcId);
                                            localById.Kill();
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }

                            RunExternalExe(filename: @strFileDownPath, useShellExcute: true);
                        }
                    }
                    catch(Exception e)
                    {
                        CLog.Here().Error($"{e}");
                    }
                }
            }
        }

        // usage
        public string RunExternalExe(string filename, string arguments = null, bool useRedirectIO = false, bool useShellExcute = false)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = useShellExcute;

            if(useShellExcute) useRedirectIO = false;
            process.StartInfo.RedirectStandardError = useRedirectIO;
            process.StartInfo.RedirectStandardOutput = useRedirectIO;
            var stdOutput = new StringBuilder();
            // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data);

            string stdError = null;
            try
            {
                process.Start();
                if(useRedirectIO) 
                {
                    process.BeginOutputReadLine();
                    stdError = process.StandardError.ReadToEnd();
                }
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + ExceptionFormat(filename, arguments)+ ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(ExceptionFormat(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        private string ExceptionFormat(string filename, string arguments)
        {
            return "'" + filename + ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) + "'";
        }

        public void SetClipBoardData(int groupID, int nType, int nClipLen, byte[] ptr) => WebWindow_SetClipBoardData(_nativeWebWindow, groupID, nType, nClipLen, ptr);
        public void ProgramExit() => WebWindow_ProgramExit(_nativeWebWindow);
        public void SetTrayUse(bool useTray) => WebWindow_SetTrayUse(_nativeWebWindow, useTray);
        public void MoveWebWindowToTray() => WebWindow_MoveWebWindowToTray(_nativeWebWindow);
        public void RegStartProgram() => WebWindow_RegStartProgram(_nativeWebWindow);
        public void UnRegStartProgram() => WebWindow_UnRegStartProgram(_nativeWebWindow);

        private void OnRequestedNavigateURL(string navURI) => NavigateURLOccured?.Invoke(this, navURI);

        public event EventHandler<string> NavigateURLOccured;
    }
}
