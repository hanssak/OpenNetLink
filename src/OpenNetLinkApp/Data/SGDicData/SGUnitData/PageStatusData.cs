using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class PageStatusData
    {
        public List<HsStream> hsStreamList = null;
        public FileAddManage fileAddManage = null;
        public PageStatusData()
        {
            hsStreamList = new List<HsStream>();
            fileAddManage = new FileAddManage();
        }
        ~PageStatusData()
        {

        }
        public void FileDragListClear()
        {
            hsStreamList.Clear();
        }

        public void SetFileDragData(HsStream hs)
        {
            if (hs == null)
                return;
            hsStreamList.Add(hs);
        }
        public void SetFileDragListData(List<HsStream> hsList)
        {
            if (hsList == null)
                return;

            hsStreamList.Clear();
            hsStreamList = new List<HsStream>(hsList);
        }
        public List<HsStream> GetFileDragListData()
        {
            return hsStreamList;
        }

        public FileAddManage GetFileAddManage()
        {
            return fileAddManage;
        }

        public static string GetRMFIlePath()
        {
            string strFilePath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var pathWithEnv = @"%USERPROFILE%\AppData\LocalLow\HANSSAK\RList\RList.txt";
                strFilePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            }
            else
            {
                // 윈도우를 제외한 다른 환경에서 경로 설정 로직 필요
                strFilePath = "/var/tmp/sgateContext.info";
            }
            return strFilePath;
        }
    }
}
