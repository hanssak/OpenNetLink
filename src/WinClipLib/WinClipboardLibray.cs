using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;

namespace WinClipLib
{

    delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
    public class WinClipboardLibray
    {
        const int HOTKEY_ID = 31197; //Any number to use to identify the hotkey instance
        const UInt32 WM_HOTKEY = 0x0312;

        public static IntPtr g_hWnd = IntPtr.Zero;

        //byte[] ClipData = null;

        public delegate void RecvHotKeyEvent(int groupID);
        public static event RecvHotKeyEvent regHotKeyEvent = null;

        public WinClipboardLibray()
        {
            create();
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, char vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        public void RegHotKey(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode)
        {
            int regID = HOTKEY_ID + groupID;
            KeyModifiers fsModifiers = KeyModifiers.None;
            if (bAlt)
                fsModifiers |= KeyModifiers.Alt;             // Alt 키 조합 (0x0001)
            if (bControl)
                fsModifiers |= KeyModifiers.Control;         // Control 키 조합 (0x0002)
            if (bShift)
                fsModifiers |= KeyModifiers.Shift;           // Shift 키 조합 (0x0004)
            if (bWin)
                fsModifiers |= KeyModifiers.Windows;			// Window 키 조합 (0x0008)

            bool bRet = RegisterHotKey(g_hWnd, regID, fsModifiers, chVKCode);
            uint error = GetLastError();
        }

        public void UnRegHotKey(int groupID)
        {
            int regID = HOTKEY_ID + groupID;
            bool bRet = UnregisterHotKey(g_hWnd, regID);
            uint error = GetLastError(); 
        }

        public void RegHotKeyNetOver(int groupID, bool bAlt, bool bControl, bool bShift, bool bWin, char chVKCode, int nIdx)
        {
            nIdx++;
            int regID = HOTKEY_ID + groupID + nIdx*100;
            KeyModifiers fsModifiers = KeyModifiers.None;
            if (bAlt)
                fsModifiers |= KeyModifiers.Alt;             // Alt 키 조합 (0x0001)
            if (bControl)
                fsModifiers |= KeyModifiers.Control;         // Control 키 조합 (0x0002)
            if (bShift)
                fsModifiers |= KeyModifiers.Shift;           // Shift 키 조합 (0x0004)
            if (bWin)
                fsModifiers |= KeyModifiers.Windows;			// Window 키 조합 (0x0008)

            bool bRet = RegisterHotKey(g_hWnd, regID, fsModifiers, chVKCode);
            uint error = GetLastError();
        }

        public void UnRegHotKeyNetOver(int groupID, int nIdx)
        {
            nIdx++;
            int regID = HOTKEY_ID + groupID + nIdx * 100;
            bool bRet = UnregisterHotKey(g_hWnd, regID);
            uint error = GetLastError();
        }


        const UInt32 WS_OVERLAPPEDWINDOW = 0xcf0000;
        const UInt32 WS_VISIBLE = 0x10000000;
        const UInt32 CS_USEDEFAULT = 0x80000000;
        const UInt32 CS_DBLCLKS = 8;
        const UInt32 CS_VREDRAW = 1;
        const UInt32 CS_HREDRAW = 2;
        const UInt32 COLOR_WINDOW = 5;
        const UInt32 COLOR_BACKGROUND = 1;
        const UInt32 IDC_CROSS = 32515;
        const UInt32 WM_DESTROY = 2;
        const UInt32 WM_PAINT = 0x0f;
        const UInt32 WM_LBUTTONUP = 0x0202;
        const UInt32 WM_LBUTTONDBLCLK = 0x0203;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct WNDCLASSEX
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }


        private WndProc delegWndProc = myWndProc;

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyWindow(IntPtr hWnd);


        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(
           int dwExStyle,
           UInt16 regResult,
           //string lpClassName,
           string lpWindowName,
           UInt32 dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
        static extern System.UInt16 RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("user32.dll")]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern void PostQuitMessage(int nExitCode);

        //[DllImport("user32.dll")]
        //static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
        //   uint wMsgFilterMax);

        [DllImport("user32.dll")]
        static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
        [DllImport("user32")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        public enum ClipDataFormat
        {
            CF_TEXT = 1,
            CF_BITMAP,         
            CF_METAFILEPICT,   
            CF_SYLK,           
            CF_DIF,           
            CF_TIFF,          
            CF_OEMTEXT,       
            CF_DIB,         
            CF_PALETTE,        
            CF_PENDATA,      
            CF_RIFF,         
            CF_WAVE,        
            CF_UNICODETEXT,    
            CF_ENHMETAFILE,   
            CF_HDROP,         
            CF_LOCALE,       
            CF_DIBV5           
        }

        internal bool create()
        {
            WNDCLASSEX wind_class = new WNDCLASSEX();
            wind_class.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            wind_class.style = (int)(CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS); //Doubleclicks are active
            wind_class.hbrBackground = (IntPtr)COLOR_BACKGROUND + 1; //Black background, +1 is necessary
            wind_class.cbClsExtra = 0;
            wind_class.cbWndExtra = 0;
            //wind_class.hInstance = Marshal.GetHINSTANCE(this.GetType().Module); ;// alternative: Process.GetCurrentProcess().Handle;
            //wind_class.hInstance = Type.GetTypeFromCLSID();
            wind_class.hIcon = IntPtr.Zero;
            wind_class.hCursor = LoadCursor(IntPtr.Zero, (int)IDC_CROSS);// Crosshair cursor;
            wind_class.lpszMenuName = null;
            wind_class.lpszClassName = "WinClipboard";
            wind_class.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(delegWndProc);
            wind_class.hIconSm = IntPtr.Zero;
            ushort regResult = RegisterClassEx(ref wind_class);

            if (regResult == 0)
            {
                uint error = GetLastError();
                return false;
            }
            string wndClass = wind_class.lpszClassName;

            //The next line did NOT work with me! When searching the web, the reason seems to be unclear! 
            //It resulted in a zero hWnd, but GetLastError resulted in zero (i.e. no error) as well !!??)
            //IntPtr hWnd = CreateWindowEx(0, wind_class.lpszClassName, "MyWnd", WS_OVERLAPPEDWINDOW | WS_VISIBLE, 0, 0, 30, 40, IntPtr.Zero, IntPtr.Zero, wind_class.hInstance, IntPtr.Zero);

            //This version worked and resulted in a non-zero hWnd
            g_hWnd = CreateWindowEx(0, regResult, "Hello Win32", WS_OVERLAPPEDWINDOW | WS_VISIBLE, 0, 0, 300, 400, IntPtr.Zero, IntPtr.Zero, wind_class.hInstance, IntPtr.Zero);

            if (g_hWnd == ((IntPtr)0))
            {
                uint error = GetLastError();
                return false;
            }

            ShowWindow(g_hWnd, 0);   // 0 -> SW_HIDE
            return true;
        }
        private static IntPtr myWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                // All GUI painting must be done here
                case WM_PAINT:
                    break;

                case WM_DESTROY:
                    DestroyWindow(hWnd);

                    break;

                case WM_HOTKEY:
                    UInt32 nHotKeyID = (UInt32)wParam;
                    int groupID = (int)( nHotKeyID - HOTKEY_ID);
                    regHotKeyEvent(groupID);
                    break;

                default:
                    break;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void SetRecvHotKeyEvent(RecvHotKeyEvent recvHotKeyEvent)
        {
            regHotKeyEvent = recvHotKeyEvent;
        }
    }
}
