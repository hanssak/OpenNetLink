using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenNetLinkApp.PageEvent
{
    public class ConfigArgs : EventArgs
    {
        public int result { get; set; }
        public string strDownLoad { get; set; }
        public int ScreenTime { get; set; }
    }
    public class FileAndClipDayArgs : EventArgs
    {
        public int result { get; set; }
        public string user_id { get; set; }
        public Int64 Size { get; set; }
        public int Count { get; set; }
    }
    public class ApproveActionEventArgs : EventArgs
    {
        public int result { get; set; }
        public string strTransSeq { get; set; }
        public string strTitle { get; set; }
        public int Action { get; set; }
        public int ApproveKind { get; set; }
        public int ApproveUserKind { get; set; }
    }
    public class AptAndVirusEventArgs : EventArgs
    {
        public int result { get; set; }
        public string strTransSeq { get; set; }
        public string strTitle { get; set; }
        public string strMsg { get; set; }
    }
    public class RecvClipEventArgs : EventArgs
    {
        public byte[] ClipData { get; set; }
        public int ClipDataSize { get; set; }
        public int nDataType { get; set; }
    }
    public class PageEventArgs : EventArgs
    {
        public string strMsg { get; set; }
        public int result { get; set; }
        public int count { get; set; }
    }

    public delegate void SvrEvent(int groupid);

    public delegate void SideBarEvent(int groupid, PageEventArgs e);
    // 로그인
    public delegate void LoginEvent(int groupid, PageEventArgs e);

    // 파일 전송 진행 이벤트 
    public delegate void FileSendProgressEvent(int groupid, PageEventArgs e);
    // 파일 수신 진행 이벤트
    public delegate void FileRecvProgressEvent(int groupid, PageEventArgs e);

    // 파일 미리보기 수신 진행 이벤트
    public delegate void FilePrevProgressEvent(int groupid, PageEventArgs e);

    // 전송관리 
    public delegate void TransSearchEvent(int groupid, PageEventArgs e);
    public delegate void TransSearchCountEvent(int groupid, PageEventArgs e);
    public delegate void TransCancelEvent(int groupid, PageEventArgs e);
        
    // 전송관리 상세보기 전송취소
    public delegate void TransDetailCancelEvent(int groupid, PageEventArgs e);

    // 결재관리
    public delegate void ApprSearchEvent(int groupid, PageEventArgs e);
    public delegate void ApprSearchCountEvent(int groupid, PageEventArgs e);
    public delegate void ApprBatchEvent(int groupid, PageEventArgs e);

    // 결재관리 상세보기 전송취소
    public delegate void ApprDetailApproveEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailRejectEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailFilePrevEvent(int groupid, PageEventArgs e);

    // 상세보기
    public delegate void DetailSearchEvent(int groupid, PageEventArgs e);

    // 같은 부서 결재라인 조회 
    public delegate void DeptApprLineSearchEvent(int groupid, PageEventArgs e);

    // 타 부서 결재라인 조회
    public delegate void DeptApprLineReflashEvent(int groupid, PageEventArgs e);

    // 마우스 우클릭 이벤트 헤더로 알림.
    public delegate void AddFileRMHeaderEvent(int groupid, PageEventArgs e);

    // 마우스 우클릭 이벤트 수신.
    public delegate void AddFileRMEvent(int groupid, PageEventArgs e);

    // 클립보드 데이터 수신
    public delegate void RecvClipEvent(int groupid, RecvClipEventArgs e);

    // 마우스 우클릭 파일 추가 이벤트
    public delegate void RMouseFileAddEvent(int groupid);

    // 공통 서버 노티 이벤트.
    public delegate void ServerNotiEvent(int groupid, eCmdList cmd, PageEventArgs e);

    // 바이러스 또는 APT 노티 이벤트.
    public delegate void APTAndVirusNotiEvent(int groupid, eCmdList cmd, AptAndVirusEventArgs e);
    //public delegate void APTAndVirusNotiEvent(eCmdList cmd, AptAndVirusEventArgs e);

    // 사용사 결재완료 노티 이벤트
    public delegate void ApproveActionNotiEvent(int groupid, eCmdList cmd, ApproveActionEventArgs e);

    // 사용된 일일 파일 전송량 노티
    public delegate void UseDayFileNotiEvent(int groupid, FileAndClipDayArgs e);

    // 사용된 일일 클립보드 전송량 노티
    public delegate void UseDayClipNotiEvent(int groupid, FileAndClipDayArgs e);

    // 로그아웃 노티
    public delegate void LogoutNotiEvent(int groupid, PageEventArgs e);

    // 화면잠금 해제 노티
    public delegate void ScreenLockClearNotiEvent(int groupid, PageEventArgs e);

    // 일일 사용량 정보 Footer 노티
    public delegate void DayInfoFooterNotiEvent(int groupid);

    // 다른 razor 화면에서 일일 파일 사용량 정보 Change 노티
    public delegate void DayFileChangeNotiEvent(int groupid);

    // 다른 razor 화면에서 일일 클립보드 사용량 정보 Change 노티
    public delegate void DayClipChangeNotiEvent(int groupid);

    // 패스워드 변경 결과 노티
    public delegate void ChangePassWDNotiEvent(int groupid, PageEventArgs e);

    // 화면잠금 초기 설정 노티
    public delegate void ScreenTimeInitNotiEvent(int groupid, ConfigArgs e);
    // 화면잠금 시간 변경 결과 노티
    public delegate void ScreenTimeChangeNotiEvent(int groupid, ConfigArgs e);
    // 수신폴더 변경 결과 노티
    public delegate void RecvFolderChangeNotiEvent(int groupid, ConfigArgs e);

    // 오프라인 노티 
    public delegate void OffLineNotiEvent(int groupid);

    // 파일 검사 delegate
    public delegate void FileExamEvent(int per, string strFileName);

    // 로그인 후 오른쪽 사이드바 환경설정 노티
    public delegate void CtrlSideEvent();

    // 업데이트 노티
    public delegate void ClientUpgradeEvent(PageEventArgs e);
    // 업데이트 실행
    public delegate void ClientUpgradeExeEvent();

    // 대쉬보드 조회 카운트 노티.
    public delegate void DashBoardCountEvent(int groupid, PageEventArgs e);

    // 대쉬보드 전송요청 카운트 노티.
    public delegate void DashBoardTransReqCountEvent(int groupid, PageEventArgs e);
    // 대쉬보드 승인대기 카운트 노티.
    public delegate void DashBoardApprWaitCountEvent(int groupid, PageEventArgs e);
    // 대쉬보드 승인 카운트 노티.
    public delegate void DashBoardApprConfirmCountEvent(int groupid, PageEventArgs e);
    // 대쉬보드 반려 카운트 노티.
    public delegate void DashBoardApprRejectCountEvent(int groupid, PageEventArgs e);

    // 패스워드 변경 날짜 조회 결과 노티.
    public delegate void PasswdChgDayEvent(int groupid, PageEventArgs e);

    // 공지사항 내용 조회 결과 노티.
    public delegate void BoardNotiSearchEvent(int groupid, PageEventArgs e);
}

namespace OpenNetLinkApp.PageEvent
{
    public class SGPageEvent
    {
        // public event LoginEvent LoginResult_Event;

        public Dictionary<int, SvrEvent> DicSvrEvent = new Dictionary<int, SvrEvent>();         // 3436 이벤트 노티

        public Dictionary<int, LoginEvent> DicLoginEvent = new Dictionary<int, LoginEvent>(); // 로그인

        public Dictionary<int, FileSendProgressEvent> DicFileSendProgressEvent = new Dictionary<int, FileSendProgressEvent>();          // 파일 전송 Progress 이벤트
        public Dictionary<int, FileRecvProgressEvent> DicFileRecvProgressEvent = new Dictionary<int, FileRecvProgressEvent>();          // 파일 수신 Progress 이벤트
        public Dictionary<int, FilePrevProgressEvent> DicFilePrevProgressEvent = new Dictionary<int, FilePrevProgressEvent>();          // 파일 미리보기 수신 Progress 이벤트.

        public Dictionary<int, TransSearchEvent> DicTransSearchEvent = new Dictionary<int, TransSearchEvent>(); // 전송관리 조회
        public Dictionary<int, TransSearchCountEvent> DicTransSearchCountEvent = new Dictionary<int, TransSearchCountEvent>(); // 전송관리 조회 데이터 Count
        public Dictionary<int, TransCancelEvent> DicTransCancelEvent = new Dictionary<int, TransCancelEvent>(); // 전송관리 전송취소
                
        public Dictionary<int, TransDetailCancelEvent> DicTransDetailCancelEvent = new Dictionary<int, TransDetailCancelEvent>(); // 전송상세보기 전송취소.

        public Dictionary<int, ApprSearchEvent> DicApprSearchEvent = new Dictionary<int, ApprSearchEvent>();         // 결재관리 조회
        public Dictionary<int, ApprSearchCountEvent> DicApprSearchCountEvent = new Dictionary<int, ApprSearchCountEvent>();         // 결재관리 조회 데이터 Count.
        public Dictionary<int, ApprBatchEvent> DicApprBatchEvent = new Dictionary<int, ApprBatchEvent>();      // 일괄 결재관리 (승인/반려)

        public Dictionary<int, ApprDetailApproveEvent> DicApprDetailApproveEvent = new Dictionary<int, ApprDetailApproveEvent>();       // 결재상세보기 승인
        public Dictionary<int, ApprDetailRejectEvent> DicApprDetailRejectEvent = new Dictionary<int, ApprDetailRejectEvent>();          // 결재상세보기 반려
        public Dictionary<int, ApprDetailFilePrevEvent> DicApprDetailFilePrevEvent = new Dictionary<int, ApprDetailFilePrevEvent>();              // 결재상세보기 미리보기

        public Dictionary<int, DetailSearchEvent> DicDetailSearchEvent = new Dictionary<int, DetailSearchEvent>();                      // 상세보기 조회

        public Dictionary<int, DeptApprLineSearchEvent> DicDeptApprLineSearchEvent = new Dictionary<int, DeptApprLineSearchEvent>();    // 같은 부서 결재라인 조회 
        public Dictionary<int, DeptApprLineReflashEvent> DicDeptApprLineReflashEvent = new Dictionary<int, DeptApprLineReflashEvent>();    // 타 부서 결재라인 조회 

        public Dictionary<int, AddFileRMEvent> DicAddFileRMEvent = new Dictionary<int, AddFileRMEvent>();                                   // 마우스 우클릭 이벤트 수신.
        public AddFileRMHeaderEvent AddRMHeaderEvent;

        public Dictionary<int, RecvClipEvent> DicRecvClipEvent = new Dictionary<int, RecvClipEvent>();                                      // 클립보드 데이터 수신 이벤트 

        public Dictionary<int, RMouseFileAddEvent> DicRMFileAddEvent = new Dictionary<int, RMouseFileAddEvent>();                                   //  마우스 우클릭 파일 추가 이벤트.

        public ServerNotiEvent SNotiEvent;                                                                                                          // 공통 서버 노티 이벤트

        public APTAndVirusNotiEvent AptAndVirusEvent;                                                                                               // 바이러스 노티 이벤트
        //public Dictionary<int, APTAndVirusNotiEvent> DicAptAndVirusEvent = new Dictionary<int, APTAndVirusNotiEvent>();                             // 바이러스 노티 이벤트 

        public ApproveActionNotiEvent ApprActionEvent;

        public Dictionary<int, UseDayFileNotiEvent> DicUseDayFileEvent = new Dictionary<int, UseDayFileNotiEvent>();                                   // 사용된 일일 파일 전송량 노티 이벤트
        public Dictionary<int, UseDayClipNotiEvent> DicUseDayClipEvent = new Dictionary<int, UseDayClipNotiEvent>();                                   // 사용된 일일 클립보드 전송량 노티 이벤트

        public LogoutNotiEvent LogoutEvent;                                                                                                     // 로그아웃 노티 이벤트
        public ScreenLockClearNotiEvent ScreenLockClearEvent;                                                                                            // 화면잠금 해제 노티 이벤트

        public Dictionary<int, DayInfoFooterNotiEvent> DicDayInfoFooterEvent = new Dictionary<int, DayInfoFooterNotiEvent>();                       // 일일 사용량 정보 Footer 노티

        public Dictionary<int, DayFileChangeNotiEvent> DicDayFileChangeEvent = new Dictionary<int, DayFileChangeNotiEvent>();                       // 다른 razor 화면에서 일일 파일 사용량 정보 Change 노티
        public Dictionary<int, DayClipChangeNotiEvent> DicDayClipChangeEvent = new Dictionary<int, DayClipChangeNotiEvent>();                       // 다른 razor 화면에서 일일 클립보드 사용량 정보 Change 노티

        public ChangePassWDNotiEvent ChgPassWDEvent;                                                                                                     // 패스워드 변경 결과 노티
        public ScreenTimeChangeNotiEvent ScrLockTimeChgEvent;                                                                                            // 화면잠금 시간 변경 결과 노티
        public ScreenTimeInitNotiEvent ScrLockTimeInitEvent;                                                                                            // 로그인 후 화면잠금 사용 및 시간 결과 노티
        public RecvFolderChangeNotiEvent RecvFolderChgEvent;                                                                                             // 수신폴더 변경 결과 노티

        public OffLineNotiEvent OfflineNotiEvent;                                                                                                           // 오프라인 노티.                                                                                                      // 오프라인 노티


        public Dictionary<int, FileExamEvent> DicFileExamEvent = new Dictionary<int, FileExamEvent>();                                                  // 파일 검사 노티.

        public CtrlSideEvent ctrlSideEvent;

        public ClientUpgradeEvent ClientUpdate;                                                                                                         // 업데이트 노티
        public ClientUpgradeExeEvent ClientUpgreadeExe;                                                                                                 // 업데이트 실행
                                                                                                                                                        // 대쉬보드 전송요청 카운트 노티.

        public Dictionary<int, DashBoardCountEvent> DicDashBoardCountEvent = new Dictionary<int, DashBoardCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardTransReqCountEvent> DicDashBoardTransReqCountEvent = new Dictionary<int, DashBoardTransReqCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardApprWaitCountEvent> DicDashBoardApprWaitCountEvent = new Dictionary<int, DashBoardApprWaitCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardApprConfirmCountEvent> DicDashBoardApprConfirmCountEvent = new Dictionary<int, DashBoardApprConfirmCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardApprRejectCountEvent> DicDashBoardApprRejectCountEvent = new Dictionary<int, DashBoardApprRejectCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.

        public Dictionary<int, PasswdChgDayEvent> DicPasswdChgDayEvent = new Dictionary<int, PasswdChgDayEvent>();                                        // 패스워드 변경 날짜 조회 결과 노티.

        public Dictionary<int, BoardNotiSearchEvent> DicBoardNotiSearchEvent = new Dictionary<int, BoardNotiSearchEvent>();                               // 공지사항 내용 조회 결과 노티.

        public SGPageEvent()
        {

        }
        ~SGPageEvent()
        {

        }

        public void SetSvrEventAdd(int groupid, SvrEvent e)
        {
            DicSvrEvent[groupid] = e;
        }
        public SvrEvent GetSvrEvent(int groupid)
        {
            SvrEvent e = null;
            if (DicSvrEvent.TryGetValue(groupid, out e) == true)
                e = DicSvrEvent[groupid];
            return e;
        }

        public void SetLoginEventAdd(int groupid, LoginEvent e)
        {
            DicLoginEvent[groupid] = e;
        }
        public LoginEvent GetLoginEvent(int groupid)
        {
            LoginEvent e = null;
            if (DicLoginEvent.TryGetValue(groupid,out e) == true)
                e = DicLoginEvent[groupid];
            return e;
        }

        public void SetTransSearchEventAdd(int groupid, TransSearchEvent e)
        {
            DicTransSearchEvent[groupid] = e;
        }
        public TransSearchEvent GetTransSearchEvent(int groupid)
        {
            TransSearchEvent e = null;
            if (DicTransSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicTransSearchEvent[groupid];
            return e;
        }

        public void SetTransCancelEventAdd(int groupid, TransCancelEvent e)
        {
            DicTransCancelEvent[groupid] = e;
        }
        public TransCancelEvent GetTransCancelEvent(int groupid)
        {
            TransCancelEvent e = null;
            if (DicTransCancelEvent.TryGetValue(groupid, out e) == true)
                e = DicTransCancelEvent[groupid];
            return e;
        }

        public void SetTransDetailCancelEventAdd(int groupid, TransDetailCancelEvent e)
        {
            DicTransDetailCancelEvent[groupid] = e;
        }
        public TransDetailCancelEvent GetTransDetailCancelEvent(int groupid)
        {
            TransDetailCancelEvent e = null;
            if (DicTransDetailCancelEvent.TryGetValue(groupid, out e) == true)
                e = DicTransDetailCancelEvent[groupid];
            return e;
        }

        public void SetApprSearchEventAdd(int groupid, ApprSearchEvent e)
        {
            DicApprSearchEvent[groupid] = e;
        }
        public ApprSearchEvent GetApprSearchEvent(int groupid)
        {
            ApprSearchEvent e = null;
            if (DicApprSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicApprSearchEvent[groupid];
            return e;
        }

        public void SetApprBatchEvent(int groupid, ApprBatchEvent e)
        {
            DicApprBatchEvent[groupid] = e;
        }
        public ApprBatchEvent GetApprBatchEvent(int groupid)
        {
            ApprBatchEvent e = null;
            if (DicApprBatchEvent.TryGetValue(groupid, out e) == true)
                e = DicApprBatchEvent[groupid];
            return e;
        }

        public void SetApprDetailApproveEvent(int groupid, ApprDetailApproveEvent e)
        {
            DicApprDetailApproveEvent[groupid] = e;
        }
        public ApprDetailApproveEvent GetApprDetailApproveEvent(int groupid)
        {
            ApprDetailApproveEvent e = null;
            if (DicApprDetailApproveEvent.TryGetValue(groupid, out e) == true)
                e = DicApprDetailApproveEvent[groupid];
            return e;
        }

        public void SetApprDetailRejectEvent(int groupid, ApprDetailRejectEvent e)
        {
            DicApprDetailRejectEvent[groupid] = e;
        }
        public ApprDetailRejectEvent GetApprDetailRejectEvent(int groupid)
        {
            ApprDetailRejectEvent e = null;
            if (DicApprDetailRejectEvent.TryGetValue(groupid, out e) == true)
                e = DicApprDetailRejectEvent[groupid];
            return e;
        }

        public void SetApprDetailFilePrevEvent(int groupid, ApprDetailFilePrevEvent e)
        {
            DicApprDetailFilePrevEvent[groupid] = e;
        }
        public ApprDetailFilePrevEvent GetApprDetailFilePrevEvent(int groupid)
        {
            ApprDetailFilePrevEvent e = null;
            if (DicApprDetailFilePrevEvent.TryGetValue(groupid, out e) == true)
                e = DicApprDetailFilePrevEvent[groupid];
            return e;
        }

        public void SetTransSearchCountEventAdd(int groupid, TransSearchCountEvent e)
        {
            DicTransSearchCountEvent[groupid] = e;
        }
        public TransSearchCountEvent GetTransSearchCountEvent(int groupid)
        {
            TransSearchCountEvent e = null;
            if (DicTransSearchCountEvent.TryGetValue(groupid, out e) == true)
                e = DicTransSearchCountEvent[groupid];
            return e;
        }
        public void SetApprSearchCountEventAdd(int groupid, ApprSearchCountEvent e)
        {
            DicApprSearchCountEvent[groupid] = e;
        }
        public ApprSearchCountEvent GetApprSearchCountEvent(int groupid)
        {
            ApprSearchCountEvent e = null;
            if (DicApprSearchCountEvent.TryGetValue(groupid, out e) == true)
                e = DicApprSearchCountEvent[groupid];
            return e;
        }

        public void SetDetailSearchEventAdd(int groupid, DetailSearchEvent e)
        {
            DetailSearchEvent temp = null;
            if(DicDetailSearchEvent.TryGetValue(groupid,out temp))
                DicDetailSearchEvent.Remove(groupid);
            DicDetailSearchEvent[groupid] = e;
        }
        public DetailSearchEvent GetDetailSearchEvent(int groupid)
        {
            DetailSearchEvent e = null;
            if (DicDetailSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicDetailSearchEvent[groupid];
            return e;
        }

        public void SetDeptApprLineSearchEventAdd(int groupid, DeptApprLineSearchEvent e)
        {
            DeptApprLineSearchEvent temp = null;
            if (DicDeptApprLineSearchEvent.TryGetValue(groupid, out temp))
                DicDeptApprLineSearchEvent.Remove(groupid);
            DicDeptApprLineSearchEvent[groupid] = e;
        }
        public DeptApprLineSearchEvent GetDeptApprLineSearchEvent(int groupid)
        {
            DeptApprLineSearchEvent e = null;
            if (DicDeptApprLineSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicDeptApprLineSearchEvent[groupid];
            return e;
        }

        public void SetDeptApprLineReflashEventAdd(int groupid, DeptApprLineReflashEvent e)
        {
            DeptApprLineReflashEvent temp = null;
            if (DicDeptApprLineReflashEvent.TryGetValue(groupid, out temp))
                DicDeptApprLineReflashEvent.Remove(groupid);
            DicDeptApprLineReflashEvent[groupid] = e;
        }
        public DeptApprLineReflashEvent GetDeptApprLineReflashEvent(int groupid)
        {
            DeptApprLineReflashEvent e = null;
            if (DicDeptApprLineReflashEvent.TryGetValue(groupid, out e) == true)
                e = DicDeptApprLineReflashEvent[groupid];
            return e;
        }

        public void SetFileSendProgressEventAdd(int groupid, FileSendProgressEvent e)
        {
            FileSendProgressEvent temp = null;
            if (DicFileSendProgressEvent.TryGetValue(groupid, out temp))
                DicFileSendProgressEvent.Remove(groupid);
            DicFileSendProgressEvent[groupid] = e;
        }
        public FileSendProgressEvent GetFileSendProgressEvent(int groupid)
        {
            FileSendProgressEvent e = null;
            if (DicFileSendProgressEvent.TryGetValue(groupid, out e) == true)
                e = DicFileSendProgressEvent[groupid];
            return e;
        }
        public void SetFileRecvProgressEventAdd(int groupid, FileRecvProgressEvent e)
        {
            FileRecvProgressEvent temp = null;
            if (DicFileRecvProgressEvent.TryGetValue(groupid, out temp))
                DicFileRecvProgressEvent.Remove(groupid);
            DicFileRecvProgressEvent[groupid] = e;
        }
        public FileRecvProgressEvent GetFileRecvProgressEvent(int groupid)
        {
            FileRecvProgressEvent e = null;
            if (DicFileRecvProgressEvent.TryGetValue(groupid, out e) == true)
                e = DicFileRecvProgressEvent[groupid];
            return e;
        }

        public void SetFilePrevProgressEventAdd(int groupid, FilePrevProgressEvent e)
        {
            FilePrevProgressEvent temp = null;
            if (DicFilePrevProgressEvent.TryGetValue(groupid, out temp))
                DicFilePrevProgressEvent.Remove(groupid);
            DicFilePrevProgressEvent[groupid] = e;
        }
        public FilePrevProgressEvent GetFilePrevProgressEvent(int groupid)
        {
            FilePrevProgressEvent e = null;
            if (DicFilePrevProgressEvent.TryGetValue(groupid, out e) == true)
                e = DicFilePrevProgressEvent[groupid];
            return e;
        }

        public void SetRecvClipEventAdd(int groupid, RecvClipEvent e)
        {
            RecvClipEvent temp = null;
            if (DicRecvClipEvent.TryGetValue(groupid, out temp))
                DicRecvClipEvent.Remove(groupid);
            DicRecvClipEvent[groupid] = e;
        }
        public RecvClipEvent GetRecvClipEvent(int groupid)
        {
            RecvClipEvent e = null;
            if (DicRecvClipEvent.TryGetValue(groupid, out e) == true)
                e = DicRecvClipEvent[groupid];
            return e;
        }

        public void SetAddRMHeaderEventAdd(AddFileRMHeaderEvent e)
        {
            AddRMHeaderEvent = e;
        }
        public AddFileRMHeaderEvent GetAddRMHeaderEventAdd()
        {
            return AddRMHeaderEvent;
        }

        public void SetAddFileRMEventAdd(int groupid, AddFileRMEvent e)
        {
            AddFileRMEvent temp = null;
            if (DicAddFileRMEvent.TryGetValue(groupid, out temp))
                DicAddFileRMEvent.Remove(groupid);
            DicAddFileRMEvent[groupid] = e;
        }
        public AddFileRMEvent GetAddFileRMEvent(int groupid)
        {
            AddFileRMEvent e = null;
            if (DicAddFileRMEvent.TryGetValue(groupid, out e) == true)
                e = DicAddFileRMEvent[groupid];
            return e;
        }

        public void SetRMouseFileAddEventAdd(int groupid, RMouseFileAddEvent e)
        {
            RMouseFileAddEvent temp = null;
            if (DicRMFileAddEvent.TryGetValue(groupid, out temp))
                DicRMFileAddEvent.Remove(groupid);
            DicRMFileAddEvent[groupid] = e;
        }
        public RMouseFileAddEvent GetRMouseFileAddEvent(int groupid)
        {
            RMouseFileAddEvent e = null;
            if (DicRMFileAddEvent.TryGetValue(groupid, out e) == true)
                e = DicRMFileAddEvent[groupid];
            return e;
        }

        public ServerNotiEvent GetServerNotiEvent()
        {
            return SNotiEvent;
        }

        public void SetServerNotiEvent(ServerNotiEvent svrNoti)
        {
            SNotiEvent = svrNoti;
        }
        public void SetAPTAndVirusNotiEventAdd(APTAndVirusNotiEvent e)
        {
            AptAndVirusEvent = e;
        }
        public APTAndVirusNotiEvent GetAPTAndVirusNotiEvent()
        {
            return AptAndVirusEvent;
        }
        /*
        public void SetAPTAndVirusNotiEventAdd(int groupid, APTAndVirusNotiEvent e)
        {
            APTAndVirusNotiEvent temp = null;
            if (DicAptAndVirusEvent.TryGetValue(groupid, out temp))
                DicAptAndVirusEvent.Remove(groupid);
            DicAptAndVirusEvent[groupid] = e;
        }
        public APTAndVirusNotiEvent GetAPTAndVirusNotiEvent(int groupid)
        {
            APTAndVirusNotiEvent e = null;
            if (DicAptAndVirusEvent.TryGetValue(groupid, out e) == true)
                e = DicAptAndVirusEvent[groupid];
            return e;
        }
        */
        public ApproveActionNotiEvent GetApproveActionNotiEvent()
        {
            return ApprActionEvent;
        }

        public void SetApproveActionNotiEvent(ApproveActionNotiEvent apprActionNoti)
        {
            ApprActionEvent = apprActionNoti;
        }


        public void SetUseDayFileNotiEventAdd(int groupid, UseDayFileNotiEvent e)
        {
            UseDayFileNotiEvent temp = null;
            if (DicUseDayFileEvent.TryGetValue(groupid, out temp))
                DicUseDayFileEvent.Remove(groupid);
            DicUseDayFileEvent[groupid] = e;
        }
        public UseDayFileNotiEvent GetUseDayFileNotiEvent(int groupid)
        {
            UseDayFileNotiEvent e = null;
            if (DicUseDayFileEvent.TryGetValue(groupid, out e) == true)
                e = DicUseDayFileEvent[groupid];
            return e;
        }

        public void SetUseDayClipNotiEventAdd(int groupid, UseDayClipNotiEvent e)
        {
            UseDayClipNotiEvent temp = null;
            if (DicUseDayClipEvent.TryGetValue(groupid, out temp))
                DicUseDayClipEvent.Remove(groupid);
            DicUseDayClipEvent[groupid] = e;
        }
        public UseDayClipNotiEvent GetUseDayClipNotiEvent(int groupid)
        {
            UseDayClipNotiEvent e = null;
            if (DicUseDayClipEvent.TryGetValue(groupid, out e) == true)
                e = DicUseDayClipEvent[groupid];
            return e;
        }


        public LogoutNotiEvent GetLogoutNotiEvent()
        {
            return LogoutEvent;
        }

        public void SetLogoutNotiEvent(LogoutNotiEvent LogoutNoti)
        {
            LogoutEvent = LogoutNoti;
        }
        public ScreenLockClearNotiEvent GetScreenLockClearNotiEvent()
        {
            return ScreenLockClearEvent;
        }

        public void SetScreenLockClearNotiEvent(ScreenLockClearNotiEvent screenLockClearNoti)
        {
            ScreenLockClearEvent = screenLockClearNoti;
        }

        public void SetDayInfoFooterNotiEventAdd(int groupid, DayInfoFooterNotiEvent e)
        {
            DayInfoFooterNotiEvent temp = null;
            if (DicDayInfoFooterEvent.TryGetValue(groupid, out temp))
                DicDayInfoFooterEvent.Remove(groupid);
            DicDayInfoFooterEvent[groupid] = e;
        }
        public DayInfoFooterNotiEvent GetDayInfoFooterNotiEvent(int groupid)
        {
            DayInfoFooterNotiEvent e = null;
            if (DicDayInfoFooterEvent.TryGetValue(groupid, out e) == true)
                e = DicDayInfoFooterEvent[groupid];
            return e;
        }

        public void SetDayFileChangeNotiEventAdd(int groupid, DayFileChangeNotiEvent e)
        {
            DayFileChangeNotiEvent temp = null;
            if (DicDayFileChangeEvent.TryGetValue(groupid, out temp))
                DicDayFileChangeEvent.Remove(groupid);
            DicDayFileChangeEvent[groupid] = e;
        }
        public DayFileChangeNotiEvent GetDayFileChangeNotiEvent(int groupid)
        {
            DayFileChangeNotiEvent e = null;
            if (DicDayFileChangeEvent.TryGetValue(groupid, out e) == true)
                e = DicDayFileChangeEvent[groupid];
            return e;
        }

        public void SetDayClipChangeNotiEventAdd(int groupid, DayClipChangeNotiEvent e)
        {
            DayClipChangeNotiEvent temp = null;
            if (DicDayClipChangeEvent.TryGetValue(groupid, out temp))
                DicDayClipChangeEvent.Remove(groupid);
            DicDayClipChangeEvent[groupid] = e;
        }
        public DayClipChangeNotiEvent GetDayClipChangeNotiEvent(int groupid)
        {
            DayClipChangeNotiEvent e = null;
            if (DicDayClipChangeEvent.TryGetValue(groupid, out e) == true)
                e = DicDayClipChangeEvent[groupid];
            return e;
        }
        public ChangePassWDNotiEvent GetChgPassWDNotiEvent()
        {
            return ChgPassWDEvent;
        }

        public void SetChgPassWDNotiEvent(ChangePassWDNotiEvent ChangePassWDNoti)
        {
            ChgPassWDEvent = ChangePassWDNoti;
        }

        public ScreenTimeChangeNotiEvent GetScreenTimeChangeNotiEvent()
        {
            return ScrLockTimeChgEvent;
        }

        public void SetScreenTimeChangeNotiEvent(ScreenTimeChangeNotiEvent screenTimeChgNoti)
        {
            ScrLockTimeChgEvent = screenTimeChgNoti;
        }
        public ScreenTimeInitNotiEvent GetScreenTimeInitNotiEvent()
        {
            return ScrLockTimeInitEvent;
        }

        public void SetScreenTimeInitNotiEvent(ScreenTimeInitNotiEvent screenTimeInitNoti)
        {
            ScrLockTimeInitEvent = screenTimeInitNoti;
        }
        public RecvFolderChangeNotiEvent GetRecvFolderChangeNotiEvent()
        {
            return RecvFolderChgEvent;
        }

        public void SetRecvFolderChangeNotiEvent(RecvFolderChangeNotiEvent recvFolderChgNoti)
        {
            RecvFolderChgEvent = recvFolderChgNoti;
        }

        public OffLineNotiEvent GetOffLineNotiEvent()
        {
            return OfflineNotiEvent;
        }

        public void SetOffLineNotiEvent(OffLineNotiEvent offlineNoti)
        {
            OfflineNotiEvent = offlineNoti;
        }

        public void SetFileExamNotiEventAdd(int groupid, FileExamEvent e)
        {
            FileExamEvent temp = null;
            if (DicFileExamEvent.TryGetValue(groupid, out temp))
                DicFileExamEvent.Remove(groupid);
            DicFileExamEvent[groupid] = e;
        }
        public FileExamEvent GetFileExamNotiEvent(int groupid)
        {
            FileExamEvent e = null;
            if (DicFileExamEvent.TryGetValue(groupid, out e) == true)
                e = DicFileExamEvent[groupid];
            return e;
        }
        public CtrlSideEvent GetCtrlSideNotiEvent()
        {
            return ctrlSideEvent;
        }

        public void SetCtrlSideNotiEvent(CtrlSideEvent ctrlSideNoti)
        {
            ctrlSideEvent = ctrlSideNoti;
        }

        public ClientUpgradeEvent GetClientUpgradeNotiEvent()
        {
            return ClientUpdate;
        }
        public void SetClientUpgradeNotiEvent(ClientUpgradeEvent updateNoti)
        {
            ClientUpdate = updateNoti;
        }
        public ClientUpgradeExeEvent GetClientUpgradeExeNotiEvent()
        {
            return ClientUpgreadeExe;
        }
        public void SetClientUpgradeExeNotiEvent(ClientUpgradeExeEvent updateNoti)
        {
            ClientUpgreadeExe = updateNoti;
        }
        public void SetDashBoardCountEventAdd(int groupid, DashBoardCountEvent e)
        {
            DashBoardCountEvent temp = null;
            if (DicDashBoardCountEvent.TryGetValue(groupid, out temp))
                DicDashBoardCountEvent.Remove(groupid);
            DicDashBoardCountEvent[groupid] = e;
        }
        public DashBoardCountEvent GetDashBoardCountEvent(int groupid)
        {
            DashBoardCountEvent e = null;
            if (DicDashBoardCountEvent.TryGetValue(groupid, out e) == true)
                e = DicDashBoardCountEvent[groupid];
            return e;
        }

        public void SetDashBoardTransReqCountEventAdd(int groupid, DashBoardTransReqCountEvent e)
        {
            DashBoardTransReqCountEvent temp = null;
            if (DicDashBoardTransReqCountEvent.TryGetValue(groupid, out temp))
                DicDashBoardTransReqCountEvent.Remove(groupid);
            DicDashBoardTransReqCountEvent[groupid] = e;
        }
        public DashBoardTransReqCountEvent GetDashBoardTransReqCountEvent(int groupid)
        {
            DashBoardTransReqCountEvent e = null;
            if (DicDashBoardTransReqCountEvent.TryGetValue(groupid, out e) == true)
                e = DicDashBoardTransReqCountEvent[groupid];
            return e;
        }

        public void SetDashBoardApprWaitCountEventAdd(int groupid, DashBoardApprWaitCountEvent e)
        {
            DashBoardApprWaitCountEvent temp = null;
            if (DicDashBoardApprWaitCountEvent.TryGetValue(groupid, out temp))
                DicDashBoardApprWaitCountEvent.Remove(groupid);
            DicDashBoardApprWaitCountEvent[groupid] = e;
        }
        public DashBoardApprWaitCountEvent GetDashBoardApprWaitCountEvent(int groupid)
        {
            DashBoardApprWaitCountEvent e = null;
            if (DicDashBoardApprWaitCountEvent.TryGetValue(groupid, out e) == true)
                e = DicDashBoardApprWaitCountEvent[groupid];
            return e;
        }
        public void SetDashBoardApprConfirmCountEventAdd(int groupid, DashBoardApprConfirmCountEvent e)
        {
            DashBoardApprConfirmCountEvent temp = null;
            if (DicDashBoardApprConfirmCountEvent.TryGetValue(groupid, out temp))
                DicDashBoardApprConfirmCountEvent.Remove(groupid);
            DicDashBoardApprConfirmCountEvent[groupid] = e;
        }
        public DashBoardApprConfirmCountEvent GetDashBoardApprConfirmCountEvent(int groupid)
        {
            DashBoardApprConfirmCountEvent e = null;
            if (DicDashBoardApprConfirmCountEvent.TryGetValue(groupid, out e) == true)
                e = DicDashBoardApprConfirmCountEvent[groupid];
            return e;
        }
        public void SetDashBoardApprRejectCountEventAdd(int groupid, DashBoardApprRejectCountEvent e)
        {
            DashBoardApprRejectCountEvent temp = null;
            if (DicDashBoardApprRejectCountEvent.TryGetValue(groupid, out temp))
                DicDashBoardApprRejectCountEvent.Remove(groupid);
            DicDashBoardApprRejectCountEvent[groupid] = e;
        }
        public DashBoardApprRejectCountEvent GetDashBoardApprRejectCountEvent(int groupid)
        {
            DashBoardApprRejectCountEvent e = null;
            if (DicDashBoardApprRejectCountEvent.TryGetValue(groupid, out e) == true)
                e = DicDashBoardApprRejectCountEvent[groupid];
            return e;
        }
        public void SetPasswdChgDayEventAdd(int groupid, PasswdChgDayEvent e)
        {
            PasswdChgDayEvent temp = null;
            if (DicPasswdChgDayEvent.TryGetValue(groupid, out temp))
                DicPasswdChgDayEvent.Remove(groupid);
            DicPasswdChgDayEvent[groupid] = e;
        }
        public PasswdChgDayEvent GetPasswdChgDayEvent(int groupid)
        {
            PasswdChgDayEvent e = null;
            if (DicPasswdChgDayEvent.TryGetValue(groupid, out e) == true)
                e = DicPasswdChgDayEvent[groupid];
            return e;
        }
        public void SetBoardNotiSearchEventAdd(int groupid, BoardNotiSearchEvent e)
        {
            BoardNotiSearchEvent temp = null;
            if (DicBoardNotiSearchEvent.TryGetValue(groupid, out temp))
                DicBoardNotiSearchEvent.Remove(groupid);
            DicBoardNotiSearchEvent[groupid] = e;
        }
        public BoardNotiSearchEvent GetBoardNotiSearchEvent(int groupid)
        {
            BoardNotiSearchEvent e = null;
            if (DicBoardNotiSearchEvent.TryGetValue(groupid, out e) == true)
                e = DicBoardNotiSearchEvent[groupid];
            return e;
        }
    }
}
