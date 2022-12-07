using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;
using OpenNetLinkApp.PageEvent;
using HsNetWorkSGData;
using Serilog;
using System.Diagnostics;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum ePassWDType
    {
        eNone = 0,
        eINITPASSWDCHG = 1,                                 // 초기 비밀번호 변경.
        eDAYPASSWDCHG = 2,                                   // 날짜에 의한 비밀번호 변경.
        eUSERPASSWDCHG = 3                                   // 사용자에 의한 비밀번호 변경.
    }

    /// <summary>
    /// 사후결재 실시간 변경 발생 시 호출할 로그인 정보와 PageStatusData 정보 보관 클래스
    /// </summary>
    public class AfterApproveNotiData
    {
        public PageStatusData AfterApprovePageStatusData;
        public SGLoginData AfterApproveSGLoginData;

        public AfterApproveNotiData(PageStatusData getPageStatusData, SGLoginData getSGLogin)
        {
            AfterApprovePageStatusData = getPageStatusData;
            AfterApproveSGLoginData = getSGLogin;
        }
    }

    public delegate void AfterApprTimeEvent();
    public delegate void InitPassWDCHGEvent(int groupID, PageEventArgs e);                           // 초기 비밀번호 변경 결과 이벤트
    public delegate void DayPassWDCHGEvent(int groupID, PageEventArgs e);                            // 날짜 비밀번호 변경 결과 이벤트
    public delegate void UserPassWDCHGEvent(int groupID, PageEventArgs e);                          // 사용자에 의한 비밀번호 변경 결과 이벤트
    public delegate void DeleteAlramAndMessageEvent();                                  // Alram 과 Message 삭제 이벤트

    /// <summary>
    /// 매일 자정 시 데이터 새로고침
    /// </summary>
    public delegate void DayInfoRefreshEvent();

    /// <summary>
    /// 로그아웃때, FileList 제거하는 이벤트
    /// </summary>
    public delegate void LogOutFileListClearEvent();

    /// <summary>
    /// Group ID 별로 각각 관리
    /// </summary>
    public class PageStatusData
    {
        public List<HsStream> hsStreamList = null;
        public FileAddManage fileAddManage = null;

        public Dictionary<string, SGNetOverData> dicSysIdName = null;

        bool m_bAfterApprCheckHide = false;
        bool m_bAfterApprEnable = false;
        bool m_bCheckAfterApprove = false;

        // 필수결재 기능 사용UI 떴을때, 사용자가 결재로 파일전송 요청했는지 유무
        bool m_bApproveExtTransFileWithApprove = false;


        //타이머관련 변수는 PageStatusService로 이동
        //public Timer timer = null;
        //public static DateTime svrTime;
        //public static string tempRefresh;
        //public static string strDatetime = "0";
        //public static string strRefreshTemp = "0";

        //GROUP ID 별 사후결재 조건 검사 타이머
        public AfterApprTimeEvent AfterChkHideEvent;

        // 사후결재 조건 검사 타이머 
        public static AfterApprTimeEvent SNotiEvent;

        /// <summary>
        /// HEADER UI 용 노티
        /// </summary>
        public static AfterApprTimeEvent SCommonNotiEvent;

        /// <summary>
        /// 공통환경설정의 사후결재 컨트롤 체크여부 변경 노티 (공통환경설정 -> 파일전송)
        /// </summary>
        public static AfterApprTimeEvent SAfterApprControlCheckEvent;

        /// <summary>
        /// 매일 자정시 데이터 새로고침 이벤트
        /// </summary>
        public static DayInfoRefreshEvent RefreshInfoEvent;

        /// <summary>
        /// 매일 자정시 알람 또는 메세지 삭제 이벤트
        /// </summary>
        public static DeleteAlramAndMessageEvent DeleteAlramAndMessage;

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

        public bool m_bUseClipBoard = false;

        public bool m_bLoginComplete = false;

        public bool m_bFileView = true;       // true 이면 일일 파일 전송량 횟수 표시 , false 이면 일일 클립보드 전송량 횟수 표시 

        public ePassWDType m_ePassWDChgType = ePassWDType.eINITPASSWDCHG;            // 패스워드 변경 종류 ( eINITPASSWDCHG : 초기 비밀번호, eDAYPASSWDCHG: 날짜,eUSERPASSWDCHG: 사용자 )
        public InitPassWDCHGEvent m_InitPasswdChgEvent;                                // 초기 비밀번호 변경 결과 이벤트
        public DayPassWDCHGEvent m_DayPasswdChgEvent;                                  // 날짜 비밀번호 변경 결과 이벤트
        public UserPassWDCHGEvent m_UserPasswdChgEvent;                                // 사용자에 의한 비밀번호 변경 결과 이벤트

        /// <summary>
        /// 로그아웃때, 파일리스트 제거하는 event
        /// </summary>
        public LogOutFileListClearEvent m_logOutFileListClearEvent = null;


        public int m_nConnectCount = 0;                                                  // 접속상태 Count (처음 접속인지 재접속인지 여부를 확인)
        public bool m_bConnect = false;                                                   // 접속상태 ( true : 접속 중, false : 오프라인 상태)

        public bool m_bLoadApprBaseLine = false;

        public bool m_bInitApprLine = false;

        public string m_strFileSendInfo = "/Transfer";

        public bool m_bLogout;

        public string m_strBoardHash = "";

        public string m_strCurFileTransPage = "/Transfer/";

        public string m_strZipDepthInfo = "";


        private SGData sgEncData = new SGData();

        public Serilog.ILogger CLog => Serilog.Log.ForContext<PageStatusData>();

        public PageStatusData()
        {
            hsStreamList = new List<HsStream>();
            fileAddManage = new FileAddManage();
            dicSysIdName = new Dictionary<string, SGNetOverData>();
        }
        public PageStatusData(int groupID)
        {
            hsStreamList = new List<HsStream>();
            fileAddManage = new FileAddManage(groupID);
            dicSysIdName = new Dictionary<string, SGNetOverData>();
            m_strFileSendInfo += "/";
            m_strFileSendInfo += groupID.ToString();
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

        /// <summary>
        /// 기존 Stream에서 신규 Stream 항목들 '추가'
        /// </summary>
        /// <param name="listHs"></param>
        public void SetFileDragData(List<HsStream> listHs)
        {
            if (listHs == null || listHs.Count < 1)
                return;
            hsStreamList.AddRange(listHs);
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


        public void SetTargetSystemListData(Dictionary<string, SGNetOverData> dicSystemIdName)
        {
            if (dicSystemIdName == null)
                return;
            dicSysIdName.Clear();
            dicSysIdName = new Dictionary<string, SGNetOverData>(dicSystemIdName);
        }

        public Dictionary<string, SGNetOverData> GetTargetSystemListData()
        {
            return dicSysIdName;
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
            for (int i = 0; i < count; i++)
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
        public void SetAfterApproveCheck(bool bCheckAfterApprove)
        {
            m_bCheckAfterApprove = bCheckAfterApprove;
        }
        public bool GetAfterApproveCheck()
        {
            return m_bCheckAfterApprove;
        }

        #region [미사용] PageStatusService로 이동
        //public void SetSvrTime(DateTime dt)
        //{
        //    svrTime = dt;
        //    strDatetime = DateTime.Now.ToString("yyyyMMdd");
        //    if (timer == null)
        //    {
        //        timer = new Timer();
        //        timer.Interval = 1000;              // 1초
        //        timer.Elapsed += new ElapsedEventHandler(AfterApprTimer);
        //        timer.Start();
        //    }
        //}

        //public static void AfterApprTimer(object sender, ElapsedEventArgs e)
        //{
        //    svrTime = svrTime.AddSeconds(1);

        //    if ((svrTime.Minute == 00) && (svrTime.Second == 0))
        //    {
        //        //매시간 정각마다 실행
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();

        //        if (SNotiEvent != null)
        //            SNotiEvent();

        //        //매일 자정마다 '일일 전송 가능' 정보 갱신 by 2022.08.
        //        if (svrTime.Hour == 0 && RefreshInfoEvent != null)  //매일 자정마다 실행
        //            RefreshInfoEvent();

        //    }

        //    ////임시 - 이벤트 여부 확인
        //    //try
        //    //{
        //    //    string jsonString = System.IO.File.ReadAllText("wwwroot/conf/RefreshTest.txt");
        //    //    if (string.IsNullOrEmpty(jsonString))
        //    //        tempRefresh = jsonString;
        //    //    else if (tempRefresh != jsonString)
        //    //    {
        //    //        RefreshInfoEvent();
        //    //        tempRefresh = jsonString;
        //    //    }
        //    //}
        //    //catch (Exception) { }



        //    if (svrTime.Second == 0)
        //    {
        //        //매분마다 memory 사용량 증가 확인
        //        //System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
        //        //Log.Information("OpenNetLink - ##### - MemoryCheck - Current process : {0}", proc.Id);
        //        //Log.Information("OpenNetLink - ##### - MemoryCheck - Private Memory : {0} MB", proc.PrivateMemorySize64 / (1024*1024));
        //        //Log.Information("OpenNetLink - ##### - MemoryCheck - Working Set : {0} MB", proc.WorkingSet64 / (1024 * 1024));
        //    }


        //}

        //public void SetAfterApprTimeEvent(AfterApprTimeEvent afterApprTime)
        //{
        //    SNotiEvent = afterApprTime;
        //}


        //public DateTime _GetAfterApprTime()
        //{
        //    return svrTime;
        //} 
        #endregion
        /// <summary>
        /// Group id 별 로그인한 사용자의 사후정책 갱신
        /// <para>SGLoginData.GetAfterChkHide 함수 보관</para>
        /// </summary>
        /// <param name="afterApprTime"></param>
        public void SetAfterChkHideEvent(AfterApprTimeEvent afterApprTime)
        {
            AfterChkHideEvent = afterApprTime;
        }

        /// <summary>
        /// 매일 자정 데이터 새로고침
        /// </summary>
        /// <param name="refreshInfo"></param>
        public void SetDayInfoRefreshEvent(DayInfoRefreshEvent refreshInfo)
        {
            RefreshInfoEvent = refreshInfo;
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
            RemainFileSize = (DayFileMaxSize * 1024 * 1024) - DayFileUseSize;
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

            return 100 - GetPercentage(DayFileUseSize, (DayFileMaxSize * 1024 * 1024), 2);
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
        public ePassWDType GetPassWDChgType()
        {
            return m_ePassWDChgType;
        }
        public void SetPassWDChgType(ePassWDType ePassWDChgType)
        {
            m_ePassWDChgType = ePassWDChgType;
        }
        public InitPassWDCHGEvent GetInitPassWDCHGEvent()
        {
            return m_InitPasswdChgEvent;
        }
        public void SetInitPassWDCHGEvent(InitPassWDCHGEvent initPasswdChgEvent)
        {
            m_InitPasswdChgEvent = initPasswdChgEvent;
        }

        public DayPassWDCHGEvent GetDayPassWDCHGEvent()
        {
            return m_DayPasswdChgEvent;
        }
        public void SetDayPassWDCHGEvent(DayPassWDCHGEvent dayPasswdChgEvent)
        {
            m_DayPasswdChgEvent = dayPasswdChgEvent;
        }

        public UserPassWDCHGEvent GetUserPassWDCHGEvent()
        {
            return m_UserPasswdChgEvent;
        }
        public void SetUserPassWDCHGEvent(UserPassWDCHGEvent userPasswdChgEvent)
        {
            m_UserPasswdChgEvent = userPasswdChgEvent;
        }


        public LogOutFileListClearEvent GetLogoutFileListClearEvent()
        {
            return m_logOutFileListClearEvent;
        }
        public void SetLogoutFileListClearEvent(LogOutFileListClearEvent userPasswdChgEvent)
        {
            m_logOutFileListClearEvent = userPasswdChgEvent;
        }

        

        public void SetCurUserPassWD(string strPW)
        {
            sgEncData.m_DicTagData.Clear();
            sgEncData.EncAdd("CURPASSWD", strPW);
        }
        public string GetCurUserPassWD()
        {
            return sgEncData.GetTagData("CURPASSWD");
        }
        public string GetEncCurUserPassWD()
        {
            return sgEncData.GetEncTagData("CURPASSWD");
        }

        public void SetSessionKey(string strSessionKey)
        {
            sgEncData.SetSessionKey(strSessionKey);
        }

        public int GetConnectCount()
        {
            return m_nConnectCount;
        }
        public void ConnectCountAdd()
        {
            m_nConnectCount++;
        }
        public bool GetConnectStatus()
        {
            return m_bConnect;
        }
        public void SetConnectStatus(bool bConnect)
        {
            m_bConnect = bConnect;
        }

        public bool GetUseClipBoard()
        {
            return m_bUseClipBoard;
        }
        public void SetUseClipBoard(bool bUse)
        {
            m_bUseClipBoard = bUse;
        }


        public bool GetLoadApprBaseLine()
        {
            return m_bLoadApprBaseLine;
        }
        public void SetLoadApprBaseLine(bool bLoadApprBaseLine)
        {
            m_bLoadApprBaseLine = bLoadApprBaseLine;
        }
        public bool GetInitApprLine()
        {
            return m_bInitApprLine;
        }
        public void SetInitApprLine(bool bInitApprLine)
        {
            m_bInitApprLine = bInitApprLine;
        }
        public void SetCurFileSendInfo(string strFileSendInfo)
        {
            m_strFileSendInfo = strFileSendInfo;
        }
        public string GetCurFileSendInfo()
        {
            return m_strFileSendInfo;
        }
        public bool GetLogoutStatus()
        {
            return m_bLogout;
        }
        public void SetLogoutStatus(bool bLogout)
        {
            m_bLogout = bLogout;
        }
        public void SetBoardHash(string strHash)
        {
            m_strBoardHash = strHash;
        }
        public string GetBoardHash()
        {
            return m_strBoardHash;
        }

        public string GetFileTransPage()
        {
            return m_strCurFileTransPage;
        }

        public void SetFileTransPage(string strFileTransPage)
        {
            m_strCurFileTransPage = strFileTransPage;
        }


        public void SetApproveExtFileTransWithApprove(bool bUseApprove)
        {
            m_bApproveExtTransFileWithApprove = bUseApprove;
        }

        public bool GetApproveExtFileTransWithApprove()
        {
            return m_bApproveExtTransFileWithApprove;
        }


    }
}
