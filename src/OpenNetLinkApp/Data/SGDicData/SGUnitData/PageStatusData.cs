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

        public Int64 RemainFileSize = 0;
        public int RemainFileCount = 0;
        public Int64 RemainClipSize = 0;
        public int RemainClipCount = 0;

        public bool m_bLoginComplete = false;

        public bool m_bFileView = true;       // true 이면 일일 파일 전송량 횟수 표시 , false 이면 일일 클립보드 전송량 횟수 표시 

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

        public Int64 GetFileDragListTotalSize()
        {
            int count = hsStreamList.Count;
            if (count <= 0)
                return 0;

            Int64 nTotalSize = 0;
            for(int i=0;i<count;i++)
            {
                nTotalSize += hsStreamList[i].Size;
            }
            return nTotalSize;
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

        public void SetDayUseFile(Int64 fileSize, int fileCount)
        {
            DayFileUseSize = fileSize;
            DayFileUseCount = fileCount;
        }

        public void SetDayUseClip(Int64 clipSize, int clipCount)
        {
            DayClipUseSize = clipSize;
            DayClipUseCount = clipCount;
        }

        public void SetDayRemainFile(Int64 fileSize, int fileCount)
        {
            RemainFileSize = fileSize;
            RemainFileCount = fileCount;
        }

        public void SetDayRemainClip(Int64 clipSize, int clipCount)
        {
            RemainClipSize = clipSize;
            RemainClipCount = clipCount;
        }

        public Int64 GetDayRemainFileSize()
        {
            RemainFileSize = (DayFileMaxSize * 1024 * 1024) - DayFileUseSize;
            if (RemainFileSize <= 0)
                RemainFileSize = 0;
            return RemainFileSize;
        }

        public int GetDayRemainFileCount()
        {
            RemainFileCount = DayFileMaxCount - DayFileUseCount;
            if (RemainFileCount <= 0)
                RemainFileCount = 0;
            return RemainFileCount;
        }

        public Int64 GetDayRemainClipSize()
        {
            RemainClipSize = (DayClipMaxSize * 1024 * 1024) - DayClipUseSize;
            if (RemainClipSize <= 0)
                RemainClipSize = 0;
            return RemainClipSize;
        }

        public int GetDayRemainClipCount()
        {
            RemainClipCount = DayClipMaxCount - DayClipUseCount;
            if (RemainClipCount <= 0)
                RemainClipCount = 0;
            return RemainClipCount;
        }

        public string GetDayRemainFileSizeString()
        {
            RemainFileSize = (DayFileMaxSize * 1024 * 1024 )- DayFileUseSize;
            if (RemainFileSize <= 0)
                RemainFileSize = 0;
            string strRet = "";
            Int64 nRemainConvertFileSize = 0;

            if (RemainFileSize < (1024 * 1024))
            {
                nRemainConvertFileSize = RemainFileSize / 1024;
                strRet = String.Format("{0:#,0} KB", nRemainConvertFileSize);
            }
            else
            {
                nRemainConvertFileSize = RemainFileSize / 1024 / 1024;
                strRet = String.Format("{0:#,0} MB", nRemainConvertFileSize);
            }
            return strRet;
        }

        public string GetDayRemainFileCountString()
        {
            RemainFileCount = DayFileMaxCount - DayFileUseCount;
            if (RemainFileCount <= 0)
                RemainFileCount = 0;
            return RemainFileCount.ToString();
        }

        public string GetDayRemainClipSizeString()
        {
            RemainClipSize = (DayClipMaxSize * 1024 * 1024) - DayClipUseSize;
            if (RemainClipSize <= 0)
                RemainClipSize = 0;
            string strRet = "";
            Int64 nRemainConvertClipSize = 0;

            if (RemainClipSize < (1024 * 1024))
            {
                nRemainConvertClipSize = RemainClipSize / 1024;
                strRet = String.Format("{0:#,0} KB", nRemainConvertClipSize);
            }
            else
            {
                nRemainConvertClipSize = RemainClipSize / 1024 / 1024;
                strRet = String.Format("{0:#,0} MB", nRemainConvertClipSize);
            }
            return strRet;
        }

        public string GetDayRemainClipCountString()
        {
            RemainClipCount = DayClipMaxCount - DayClipUseCount;
            if (RemainClipCount <= 0)
                RemainClipCount = 0;
            return RemainClipCount.ToString();
        }

        private double GetPercentage(double value, double total, int decimalplaces)
        {
            return System.Math.Round(value * 100 / total, decimalplaces);
        }

        public double GetDayRemainFileSizePercent()
        {
            if (DayFileUseSize == 0)
                return 100;

            return 100 - GetPercentage(DayFileUseSize,(DayFileMaxSize*1024*1024), 2);
        }

        public double GetDayRemainFileCountPercent()
        {
            if (DayFileUseCount == 0)
                return 100;

            return 100 - GetPercentage(DayFileUseCount, DayFileMaxCount, 2);
        }

        public double GetDayRemainClipSizePercent()
        {
            if (DayClipUseSize == 0)
                return 100;

            return 100 - GetPercentage(DayClipUseSize, (DayClipMaxSize * 1024 * 1024), 2);
        }

        public double GetDayRemainClipCountPercent()
        {
            if (DayClipUseCount == 0)
                return 100;

            return 100 - GetPercentage(DayClipUseCount, DayClipMaxCount, 2);
        }

        public bool GetDayFileTransSizeEnable(Int64 nFileListSize)
        {
            Int64 FileTransMaxSize = GetDayFileMaxSize();
            if (FileTransMaxSize <= 0)
                return true;

            Int64 RemainFileTransSize = GetDayRemainFileSize();
            if (RemainFileTransSize < nFileListSize)
                return false;
            return true;
        }
        public bool GetDayFileTransCountEnable()
        {
            int FileTransMaxCount = GetDayFileMaxCount();
            if (FileTransMaxCount <= 0)
                return true;

            int RemainFileTransCount = GetDayRemainFileCount();
            if (RemainFileTransCount <= 0)
                return false;
            return true;
        }

        public bool GetDayClipboardSizeEnable(Int64 nClipSize)
        {
            Int64 ClipboardMaxSize = GetDayClipMaxSize();
            if (ClipboardMaxSize <= 0)
                return true;

            Int64 RemainClipSize = GetDayRemainClipSize();
            if (RemainClipSize < nClipSize)
                return false;
            return true;
        }

        public bool GetDayClipboardCountEnable()
        {
            int ClipBoardMaxCount = GetDayClipMaxCount();
            if (ClipBoardMaxCount <= 0)
                return true;

            int RemainClipCount = GetDayRemainClipCount();
            if (RemainClipCount <= 0)
                return false;
            return true;
        }

        public void SetLoginComplete(bool bLoginComplete)
        {
            m_bLoginComplete = bLoginComplete;
        }
        public bool GetLoginComplete()
        {
            return m_bLoginComplete;
        }

        public bool GetDayInfoPrev()
        {
            return m_bFileView;
        }

        public void SetDayInfoPrev(bool bFileView)
        {
            m_bFileView = bFileView;
        }

        public bool GetDayFileSizeUnLimited()
        {
            if (DayFileMaxSize == 0)
                return true;
            return false;
        }

        public bool GetDayFileCountUnLimited()
        {
            if (DayFileMaxCount == 0)
                return true;
            return false;
        }

        public bool GetDayClipSizeUnLimited()
        {
            if (DayClipMaxSize == 0)
                return true;
            return false;
        }

        public bool GetDayClipCountUnLimited()
        {
            if (DayClipMaxCount == 0)
                return true;
            return false;
        }

    }
}
