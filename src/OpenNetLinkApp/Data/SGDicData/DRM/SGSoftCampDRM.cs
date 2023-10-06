using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Events;
using AgLogManager;
using System.Collections.Generic;
using System.Linq;

namespace OpenNetLinkApp.Data.SGDicData.DRM
{
    class SGSoftCampDRM
    {
        public const string strSoftCampDrmLibName = "C:\\Windows\\DSCSLink.dll";
        [DllImport(strSoftCampDrmLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DSCSGradeEncryptFileV2(string stSource, string grade);

        public int DrmEncrypt(string source)
        {
            string grade = "0000001";

            int ret = DSCSGradeEncryptFileV2(source, grade);

            //ret == 0 암호화 실패
            //ret == 1 암호화 성공
            //ret == 2 미지원확장자
            //ret == 3 문서보안 로그아웃

            return ret;
        }
    }
}
