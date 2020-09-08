using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public delegate void AfterApprTimeEvent();                                                                                                     
    public class PageStatusData
    {
        public List<HsStream> hsStreamList = null;
        public FileAddManage fileAddManage = null;

        bool m_bAfterApprCheckHide = false;
        bool m_bAfterApprEnable = false;

        public Timer timer = null;

        public static DateTime svrTime;

        // 사후결재 조건 검사 타이머 
        public static AfterApprTimeEvent SNotiEvent;

        public Int64 DayFileMaxSize = 0;
        public int DayFileMaxCount = 0;
        public Int64 DayClipMaxSize = 0;
        public int DayClipMaxCount = 0;

        public Int64 DayFileUseSize = 0;
        public int DayFileUseCount = 0;
        public Int64 DayClipUseSize = 0;
        public int DayClipUseCount = 0;


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
        public void SetAfterApprChkHIde(bool bAfterApprCheckHide)
        {
            m_bAfterApprCheckHide = bAfterApprCheckHide;
        }
        public bool GetAfterApprChkHide()
        {
            return m_bAfterApprCheckHide;
        }
        public void SetAfterApprEnable(bool bAfterApprEnable)
        {
            m_bAfterApprEnable = bAfterApprEnable;
        }
        public bool GetAfterApprEnable()
        {
            return m_bAfterApprEnable;
        }

        public void SetSvrTime(DateTime dt)
        {
            svrTime = dt;
            timer = new Timer();
            timer.Interval = 1000;              // 1초
            timer.Elapsed += new ElapsedEventHandler(AfterApprTimer);
            timer.Start();
        }

        public static void AfterApprTimer(object sender, ElapsedEventArgs e)
        {
            svrTime = svrTime.AddSeconds(1);
            if( (svrTime.Minute==0) && (svrTime.Second==0) )
            {
                if(SNotiEvent != null)
                    SNotiEvent();
            }
        }

        public void SetAfterApprTimeEvent(AfterApprTimeEvent afterApprTime)
        {
            SNotiEvent = afterApprTime;
        }

        public DateTime GetAfterApprTime()
        {
            return svrTime;
        }

        public void SetDayFileAndClipMax(Int64 fileMaxSize, int fileMaxCount, Int64 clipMaxSize, int clipMaxCount)
        {
            DayFileMaxSize = fileMaxSize;
            DayFileMaxCount = fileMaxCount;
            DayClipMaxSize = clipMaxSize;
            DayClipMaxCount = clipMaxCount;
        }

        public Int64 GetDayFileMaxSize()
        {
            return DayFileMaxSize;
        }

        public int GetDayFileMaxCount()
        {
            return DayFileMaxCount;
        }

        public Int64 GetDayClipMaxSize()
        {
            return DayClipMaxSize;
        }

        public int GetDayClipMaxCount()
        {
            return DayClipMaxCount;
        }
    }
}
