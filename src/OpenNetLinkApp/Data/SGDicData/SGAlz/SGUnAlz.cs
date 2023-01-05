using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using AgLogManager;
using System.Collections.Generic;
using System.Linq;

namespace OpenNetLinkApp.Data.SGDicData.SGAlz
{
    class SGUnAlz
    {
#if _WINDOWS
        //dll 경로 지정
        public const string strAlzLibName = "D:\\github\\OpenNetLink\\src\\OpenNetLinkApp\\Library\\UnAlzDll.dll";
        //public const string strAlzLibName = "UnAlzDll.dll";
#elif _LINUX
        public const string strGpkiLibName = "libgpkiapi.so";
#elif _MACOSX
        public const string strGpkiLibName = "libgpkiapi.so";
#else
        public const string strGpkiLibName = "libgpkiapi.so";
#endif

#if _WINDOWS
        [DllImport(strAlzLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int UnAlzExtract(string stSource, string stDest);
        [DllImport(strAlzLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int UnAlzExtract_PassWord(string stSource, string stDest, string stPassWord);
        [DllImport(strAlzLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int UnAlzExtractW(string stSource, string stDest);
        [DllImport(strAlzLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int UnAlzExtract_PassWordW(string stSource, string stDest, string stPassWord);
        public int UnAlzExtractDll(string source, string dest)
        {
            int ret = UnAlzExtract(source, dest);

            return ret;
        }
        public int UnAlzExtract_PassWordDll(string source, string dest, string passWord)
        {
            int ret = UnAlzExtract_PassWord(source, dest, passWord);

            return ret;
        }
        public int UnAlzExtractWDll(string source, string dest)
        {
            int ret = UnAlzExtractW(source, dest);

            return ret;
        }
        public int UnAlzExtract_PassWordWDll(string source, string dest, string passWord)
        {
            int ret = UnAlzExtract_PassWordW(source, dest, passWord);

            return ret;
        }
#endif
    }
}
