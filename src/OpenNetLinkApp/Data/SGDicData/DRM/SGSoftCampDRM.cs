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
        public const string strSoftCampDrmLibName = "C:\\Windows\\DSCSLink64.dll";
        [DllImport(strSoftCampDrmLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DSCSGradeEncryptFileV2(string stSource, string grade);

        [DllImport(strSoftCampDrmLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DSCSAddDac(int nGuide, string grade);

        [DllImport(strSoftCampDrmLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DSCheckDSAgent();

        [DllImport(strSoftCampDrmLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DSCSIsEncryptedFile(string strFullFilePath);

        [DllImport(strSoftCampDrmLibName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DSCSDacEncryptFile(string strFullFilePath);

        




        public int DrmEncrypt(string source, string grade)
        {
            //string grade = "0000004";
            int ret = DSCSGradeEncryptFileV2(source, grade);

            //ret == 0 암호화 실패
            //ret == 1 암호화 성공
            //ret == 2 미지원확장자
            //ret == 3 문서보안 로그아웃

            return ret;
        }

        public bool DrmAddDac(int nGuide, string grade)
        {
            try
            {
                DSCSAddDac(nGuide, grade);
                return true;
            }
            catch(Exception ex)
            {
                Log.Logger.Here().Error($"DrmAddDac, Exception(MSG) : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DRM 사용할 수 있도록 DRM Agent가 Login 되어 있는지 유무
        /// </summary>
        /// <returns></returns>
        public bool DrmAgentisLogin()
        {

            int nStatus = -1;

            try
            {
                nStatus = DSCheckDSAgent();

                if (nStatus == 0)
                    Log.Logger.Here().Error($"DrmAgentisLogin, DS Client Agent is Not Alive! (Ret=0)");
                else if (nStatus == 1)
                    Log.Logger.Here().Error($"DrmAgentisLogin, DS Client Agent is logout Status! (Ret=1)");

                return (nStatus == 2);
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"DrmAgentisLogin, Exception(MSG) : {ex.Message}, Status : {nStatus}");
                return false;
            }
        }

        public bool DrmisEncryptFile(string strFilePath)
        {
            int nStatus = -1;

            try
            {
                nStatus = DSCSIsEncryptedFile(strFilePath);

                if (nStatus == -1)
                    Log.Logger.Here().Error($"DrmisEncryptFile, DRM CS module Load Failed : {strFilePath}");
                else if (nStatus == 0)
                    Log.Logger.Here().Error($"DrmisEncryptFile, Normal File : {strFilePath}");

                return (nStatus == 1);
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"DrmisEncryptFile, Exception(MSG) : {ex.Message}, Status : {nStatus}");
                return false;
            }
        }

        public bool DrmEncryptUsingDac(string strFilePath)
        {
            int nStatus = -1;

            try
            {
                nStatus = DSCSDacEncryptFile(strFilePath);

                if (nStatus == 3)
                    Log.Logger.Here().Error($"DrmEncryptUsingDac, DRM CS Out Status, Status : {nStatus}");
                else if (nStatus == 0)
                    Log.Logger.Here().Error($"DrmEncryptUsingDac, DRM EncryptFile Failed : {strFilePath}");

                return (nStatus == 1);
            }
            catch (Exception ex)
            {
                Log.Logger.Here().Error($"DrmEncryptUsingDac, Exception(MSG) : {ex.Message}, Status : {nStatus}");
                return false;
            }
        }


    }
}
