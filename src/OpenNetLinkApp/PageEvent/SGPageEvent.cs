using HsNetWorkSG;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using HsNetWorkSGData;
using System.Threading.Tasks;
using OpenNetLinkApp.Common;

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

    public class RecvUrlEventArgs : EventArgs
    {
        public string strUrlData { get; set; }
    }

    public class PrivacyEventArgs : EventArgs
    {
        public string TRANSKIND { get; set; }
        public string TRANSSEQ { get; set; }
        public string TITLE { get; set; }
        public string APPROVEKIND { get; set; }
        public string ERROR_CODE { get; set; }
        public string NETOVERSYSTEM { get; set; }
    }

    public class RecvDataEventArgs : PageEventArgs
    {
        public string strDataType { get; set; }

        public string strFilePath { get; set; }

    }

    public class PageEventArgs : EventArgs
    {
        public string strMsg { get; set; }
        public int result { get; set; }
        public int count { get; set; }
        public string strDummy { get; set; }
    }

    public delegate void SvrEvent(int groupid, string loginType);
    public delegate void SvrGPKIEvent(int groupid);
    public delegate void SvrGPKIRandomKeyEvent(int groupid);
    public delegate void SvrGPKICertEvent(int groupid);
    public delegate void SvrGPKIRegEvent(int groupid);
    public delegate void SideBarEvent(int groupid, PageEventArgs e);
    // 로그인
    public delegate void LoginEvent(int groupid, PageEventArgs e);
    // 중복로그인 이벤트
    public delegate void SessionDuplicateEvent(int groupid, PageEventArgs e);

    // 파일 전송 진행 이벤트 
    public delegate void FileSendProgressEvent(int groupid, PageEventArgs e);
    // 파일 수신 진행 이벤트
    public delegate void FileRecvProgressEvent(int groupid, RecvDataEventArgs e);
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

    //공통 응답 이벤트(통합해서 하나만 쓰게 수정해야 할듯) 2021/05/14 YKH
    public delegate void ResponseEvent(int groupid, SGData e);

    //대결재 관리
    public delegate void ProxySearchEvent(int groupid, SGData e);
    public delegate void CommonResultEvent(int groupid, SGData e);
    // 결재관리 상세보기 전송취소
    public delegate void ApprDetailApproveEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailRejectEvent(int groupid, PageEventArgs e);
    public delegate void ApprDetailFilePrevEvent(int groupid, PageEventArgs e);
    // 상세보기
    public delegate void DetailSearchEvent(int groupid, PageEventArgs e);
    //다운로드 카운트 응답 이벤트 정의
    public delegate void DownloadCountEvent(int groupid, SGData e);
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
    // URL 데이터 수신
    public delegate void RecvUrlEvent(int groupid, RecvUrlEventArgs e);
    // URLList 데이터 수신
    public delegate void UrlListEvent(int groupid, PageEventArgs e);
    // 마우스 우클릭 파일 추가 이벤트
    public delegate void RMouseFileAddEvent(int groupid);
    // 공통 서버 노티 이벤트.
    public delegate void ServerNotiEvent(int groupid, eCmdList cmd, PageEventArgs e);
    // 바이러스 또는 APT 노티 이벤트.
    public delegate void APTAndVirusNotiEvent(int groupid, eCmdList cmd, AptAndVirusEventArgs e);
    // 바이러스 또는 APT 노티 DB Insert 이벤트
    public delegate void APTAndVirusNotiDBInsert(int groupid, eCmdList cmd, AptAndVirusEventArgs e);
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
    // URLRedirection 사용 설정에 대한 노티.1 - redraw용도
    public delegate void UrlRedirectionSettingNotiEvent(int groupid);  // string strGroupidMenu
    // URLRedirection 사용 설정에 대한 노티.2 - watcher Thread에게 변경된 정책을 전달하기 위함
    public delegate void UrlRedirectionPolicySetNotiEvent(int groupid, bool isChangedfromNetLib);
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
    public delegate void ClientUpgradeEvent(int groupid, PageEventArgs e);
    // 업데이트 실행
    public delegate void ClientUpgradeExeEvent(int gouprid);
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
    // 공지사항 내용 조회 후 대쉬보드 화면 갱신 노티
    public delegate void BoardNotiAfterDashBoardEvent(int groupid);
    // 공지사항 내용 조회 후 전체 공지사항 보기 화면 갱신 노티
    public delegate void BoardNotiAfterTotalBoardEvent(int groupid);
    // 알람 노티 수신 후 대쉬보드 화면 갱신 노티
    public delegate void AlarmNotiAfterDashBoardEvent(int groupid);
    // 노티 수신 후  전체 메시지 화면 갱신 노티
    public delegate void NotiAfterTotalMsgEvent();
    // 노티 수신 후 전체 알람 화면 갱신 노티
    public delegate void NotiAfterTotalAlarmEvent();
    // 로그인 후 SGSideBar 화면 갱신 노티.
    public delegate void LoginAfterSGSideBarEvent(int groupid);
    // 로그인 후 SGHeaderUI 화면 갱신 노티.
    public delegate void LoginAfterSGHeaderUIEvent(int groupid);
    // 3436 을 통한 GPKI CN 등록 상태 리스트 조회 결과 노티.
    public delegate void GPKICNListRecvEvent(int groupid, PageEventArgs e);
    /// <summary>
    /// 대결재 갱신에 따른 로그아웃 Event
    /// </summary>
    public delegate Task<int> SFMRefreshEvent(int groupId);
    /// <summary>
    /// 정책 업데이트 Notify
    /// </summary>
    /// <param name="groupId"></param>
    public delegate void NotiUpdatePolicyEvent(int groupId);
    /// <summary>
    /// FileMime 정보 갱신 Event
    /// </summary>
    /// <param name="groupId"></param>
    public delegate void FileMimeRecvEvent(int groupId);

    /// <summary>
    /// OLE Mime 정보 갱신 Event
    /// </summary>
    /// <param name="groupId"></param>
    public delegate void OLEMimeRecvEvent(int groupId, SGData e);

    // 3436 을 통한 GPKI CN 등록 상태 리스트 조회 결과 노티.
    //public delegate void GPKICNListRecvEvent(int groupid, PageEventArgs e);

    //개인정보 NOTIFY Delegate
    public delegate void PrivacyNotiEvent(int groupid, SGData e);
    // 보안결재자 조회
    public delegate void SecurityApproverSearchEvent(int groupid, SGData e);

    // 로그인 후 오른쪽 사이드바 환경설정 화면갱신 노티
    public delegate void CtrlSideRefreshEvent();

    // 쿼리 카운트 공용 이벤트 Delegate
    public delegate void QueryCountEvent(int groupid, SGData e);
    // 쿼리 리스트 공용 이벤트 Delegate
    public delegate void QueryListEvent(int groupid, SGData e);
    // 쿼리 디테일 공용 이벤트 Delegate
    public delegate void QueryDetailEvent(int groupid, SGData e);
    // 레코드 체크 이벤트 Delegate
    public delegate void QueryRecordExistCheckEvent(int groupid, SGData e);
    // 이메일 전송 취소 이벤트
    public delegate void EmailSendCancelEvent(int groupid, SGData e);
    // 파일수신 error 정보주는 이벤트
    public delegate void FileRecvErrInfoEvent(int groupid, SGData e);
    // 파일 포워딩 정보주는 이벤트
    public delegate void FileForwardEvent(int groupid, SGData e);
    //공통 쿼리 처리
    public delegate void CommonQueryReciveEvent(int groupId, object[] e);
    // GenericNoti Type 정보주는 이벤트
    public delegate void GenericNotiType2Event(int groupid, SGData e);
    // 예외처리 신청한거 완료됐음 알려주는 이벤트
    public delegate void SkipFileNotiEvent(int groupid, SGData e);
    //Page Data 갱신 처리
    public delegate void PageDataRefreshEvent();

    // 부서정보 조회 요청 응답 처리    
    public delegate void DeptInfoNotiEvent(int groupId);

    // Reconnect Count Out 시에 호출하는 event
    public delegate void ReconnectCountOutEvent(int groupid); // , PageEventArgs e

    // PkiFile 보내기 위한 Event
    public delegate bool SendPkiFileEvent(int groupid, string strPcfFilePath, bool bSendPkiByFileTrans);

    /// <summary>
    /// SystemEnv Client Query 결과 Event
    /// </summary>
    /// <param name="groupid"></param>
    /// <param name="e"></param>
    public delegate void SystemEnvQueryNotiEvent(int groupid);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId"></param>
    public delegate bool UrlTypeForwardDataEvent(int groupId);

}

namespace OpenNetLinkApp.PageEvent
{
    public class SGPageEvent
    {
        #region 기존 페이지 이벤트 
        // 대결재 이벤트 
        public Dictionary<int, ProxySearchEvent> DicProxySearch = new Dictionary<int, ProxySearchEvent>(); //조회
        public Dictionary<int, CommonResultEvent> DicCommonResult = new Dictionary<int, CommonResultEvent>(); //등록,삭제

        public Dictionary<int, SvrEvent> DicSvrEvent = new Dictionary<int, SvrEvent>();         // 3436 이벤트 노티
        public Dictionary<int, SvrGPKIEvent> DicSvrGPKIEvent = new Dictionary<int, SvrGPKIEvent>();         // 3436 이벤트 노티
        public Dictionary<int, SvrGPKIRandomKeyEvent> DicSvrGPKIRandomKeyEvent = new Dictionary<int, SvrGPKIRandomKeyEvent>();         // GPKI Random Key 이벤트
        public Dictionary<int, SvrGPKICertEvent> DicSvrGPKICertEvent = new Dictionary<int, SvrGPKICertEvent>();         // GPKI Cert 이벤트
        public Dictionary<int, SvrGPKIRegEvent> DicSvrGPKIRegEvent = new Dictionary<int, SvrGPKIRegEvent>();         // GPKI Reg 이벤트

        public Dictionary<int, LoginEvent> DicLoginEvent = new Dictionary<int, LoginEvent>(); // 로그인

        /// <summary>
        /// 세션 중복 이벤트
        /// </summary>
        public Dictionary<int, SessionDuplicateEvent> DicSessionDuplicateEvent = new Dictionary<int, SessionDuplicateEvent>(); //세션중복 이벤트

        public Dictionary<int, FileSendProgressEvent> DicFileSendProgressEvent = new Dictionary<int, FileSendProgressEvent>();          // 파일 전송 Progress 이벤트
        //public Dictionary<int, FileRecvProgressEvent> DicFileRecvProgressEvent = new Dictionary<int, FileRecvProgressEvent>();          // 파일 수신 Progress 이벤트

        /// <summary>
        /// 파일 수신 Progress 이벤트 - 필요로하는 곳에서 사용
        /// </summary>
        public FileRecvProgressEvent fileRecvProgressEvent = null;

        /// <summary>
        /// 파일 수신 Progress 이벤트 : HeaderUI쪽에서만 사용
        /// </summary>
        public FileRecvProgressEvent fileRecvProgressMasterEvent = null;

        public Dictionary<int, FilePrevProgressEvent> DicFilePrevProgressEvent = new Dictionary<int, FilePrevProgressEvent>();          // 파일 미리보기 수신 Progress 이벤트.

        public Dictionary<int, TransSearchEvent> DicTransSearchEvent = new Dictionary<int, TransSearchEvent>(); // 전송관리 조회
        public Dictionary<int, TransSearchCountEvent> DicTransSearchCountEvent = new Dictionary<int, TransSearchCountEvent>(); // 전송관리 조회 데이터 Count
        public Dictionary<int, TransCancelEvent> DicTransCancelEvent = new Dictionary<int, TransCancelEvent>(); // 전송관리 전송취소

        public Dictionary<int, TransDetailCancelEvent> DicTransDetailCancelEvent = new Dictionary<int, TransDetailCancelEvent>(); // 전송상세보기 전송취소.

        public Dictionary<int, ApprSearchEvent> DicApprSearchEvent = new Dictionary<int, ApprSearchEvent>();         // 결재관리 조회
        public Dictionary<int, ApprSearchCountEvent> DicApprSearchCountEvent = new Dictionary<int, ApprSearchCountEvent>();         // 결재관리 조회 데이터 Count.
        public Dictionary<int, ApprBatchEvent> DicApprBatchEvent = new Dictionary<int, ApprBatchEvent>();      // 일괄 결재관리 (승인/반려)
        //공통으로 통일 하려고 설정 
        public Dictionary<int, ResponseEvent> DicEmailApprBatchEvent = new Dictionary<int, ResponseEvent>(); //이메일 일괄 결재 응답 이벤트 

        public Dictionary<int, ApprDetailApproveEvent> DicApprDetailApproveEvent = new Dictionary<int, ApprDetailApproveEvent>();       // 결재상세보기 승인
        public Dictionary<int, ApprDetailRejectEvent> DicApprDetailRejectEvent = new Dictionary<int, ApprDetailRejectEvent>();          // 결재상세보기 반려
        public Dictionary<int, ApprDetailFilePrevEvent> DicApprDetailFilePrevEvent = new Dictionary<int, ApprDetailFilePrevEvent>();              // 결재상세보기 미리보기

        public Dictionary<int, DetailSearchEvent> DicDetailSearchEvent = new Dictionary<int, DetailSearchEvent>();                      // 상세보기 조회
        public Dictionary<int, DownloadCountEvent> DicDownloadCountEvent = new Dictionary<int, DownloadCountEvent>();                   //다운로드 카운트 이벤트

        public Dictionary<int, DeptApprLineSearchEvent> DicDeptApprLineSearchEvent = new Dictionary<int, DeptApprLineSearchEvent>();    // 같은 부서 결재라인 조회 
        public Dictionary<int, DeptApprLineReflashEvent> DicDeptApprLineReflashEvent = new Dictionary<int, DeptApprLineReflashEvent>();    // 타 부서 결재라인 조회 

        public Dictionary<int, SecurityApproverSearchEvent> DicSecurityApproverEvent = new Dictionary<int, SecurityApproverSearchEvent>();

        public Dictionary<int, AddFileRMEvent> DicAddFileRMEvent = new Dictionary<int, AddFileRMEvent>();                                   // 마우스 우클릭 이벤트 수신.
        public AddFileRMHeaderEvent AddRMHeaderEvent;

        public Dictionary<int, RecvClipEvent> DicRecvClipEvent = new Dictionary<int, RecvClipEvent>();                                      // 클립보드 데이터 수신 이벤트 

        public Dictionary<int, RecvUrlEvent> DicServerRecvUrlEvent = new Dictionary<int, RecvUrlEvent>();                                      // Url 데이터 수신 이벤트 (Server로부터)

        public Dictionary<int, RecvUrlEvent> DicBrowserRecvUrlEvent = new Dictionary<int, RecvUrlEvent>();                                      // Url 데이터 수신 이벤트  (Browser로부터)

        public Dictionary<int, UrlListEvent> DicUrlListEvent = new Dictionary<int, UrlListEvent>();                                      // UrlLIST 데이터 수신 이벤트  (Browser로부터)


        public Dictionary<int, RMouseFileAddEvent> DicRMFileAddEvent = new Dictionary<int, RMouseFileAddEvent>();                                   //  마우스 우클릭 파일 추가 이벤트.

        public ServerNotiEvent SNotiEvent;                                                                                                          // 공통 서버 노티 이벤트

        public APTAndVirusNotiEvent AptAndVirusEvent;                                                                                               // 바이러스 노티 이벤트
        public APTAndVirusNotiDBInsert AptAndVirusDBInsertEvent;                                                                                    // 바이러스 또는 APT 노티 DB Insert 이벤트

        public ApproveActionNotiEvent ApprActionEvent;

        public Dictionary<int, UseDayFileNotiEvent> DicUseDayFileEvent = new Dictionary<int, UseDayFileNotiEvent>();                                   // 사용된 일일 파일 전송량 노티 이벤트
        public Dictionary<int, UseDayClipNotiEvent> DicUseDayClipEvent = new Dictionary<int, UseDayClipNotiEvent>();                                   // 사용된 일일 클립보드 전송량 노티 이벤트

        public LogoutNotiEvent LogoutEvent;                                                                                                     // 로그아웃 노티 이벤트
        public ScreenLockClearNotiEvent ScreenLockClearEvent;                                                                                            // 화면잠금 해제 노티 이벤트

        public Dictionary<int, DayInfoFooterNotiEvent> DicDayInfoFooterEvent = new Dictionary<int, DayInfoFooterNotiEvent>();                       // 일일 사용량 정보 Footer 노티

        public Dictionary<int, DayFileChangeNotiEvent> DicDayFileChangeEvent = new Dictionary<int, DayFileChangeNotiEvent>();                       // 다른 razor 화면에서 일일 파일 사용량 정보 Change 노티
        public Dictionary<int, DayClipChangeNotiEvent> DicDayClipChangeEvent = new Dictionary<int, DayClipChangeNotiEvent>();                       // 다른 razor 화면에서 일일 클립보드 사용량 정보 Change 노티

        public Dictionary<string, UrlRedirectionSettingNotiEvent> DicUrlRedirectionSetEvent = new Dictionary<string, UrlRedirectionSettingNotiEvent>();                       // urlredirection 설정변경에 따른 Change 노티(Redraw 목적)
        public Dictionary<int, UrlRedirectionPolicySetNotiEvent> DicUrlRedirectionUserPolicyEvent = new Dictionary<int, UrlRedirectionPolicySetNotiEvent>();                       // urlredirection 설정변경에 따른 Change 노티(watcher Thread에게 새정책전달 목적)

        public ChangePassWDNotiEvent ChgPassWDEvent;                                                                                                     // 패스워드 변경 결과 노티
        public ScreenTimeChangeNotiEvent ScrLockTimeChgEvent;                                                                                            // 화면잠금 시간 변경 결과 노티
        public ScreenTimeInitNotiEvent ScrLockTimeInitEvent;                                                                                            // 로그인 후 화면잠금 사용 및 시간 결과 노티
        public RecvFolderChangeNotiEvent RecvFolderChgEvent;                                                                                             // 수신폴더 변경 결과 노티

        public OffLineNotiEvent OfflineNotiEvent;                                                                                                           // 오프라인 노티.                                                                                                      // 오프라인 노티


        public Dictionary<int, FileExamEvent> DicFileExamEvent = new Dictionary<int, FileExamEvent>();                                                  // 파일 검사 노티.

        public CtrlSideEvent ctrlSideEvent;
        public CtrlSideRefreshEvent ctrlSideRefreshEvent;

        public ClientUpgradeEvent ClientUpdate;                                                                                                         // 업데이트 노티
        public ClientUpgradeExeEvent ClientUpgreadeExe;                                                                                                 // 업데이트 실행
                                                                                                                                                        // 대쉬보드 전송요청 카운트 노티.

        public Dictionary<int, DashBoardCountEvent> DicDashBoardCountEvent = new Dictionary<int, DashBoardCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardTransReqCountEvent> DicDashBoardTransReqCountEvent = new Dictionary<int, DashBoardTransReqCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardApprWaitCountEvent> DicDashBoardApprWaitCountEvent = new Dictionary<int, DashBoardApprWaitCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardApprConfirmCountEvent> DicDashBoardApprConfirmCountEvent = new Dictionary<int, DashBoardApprConfirmCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.
        public Dictionary<int, DashBoardApprRejectCountEvent> DicDashBoardApprRejectCountEvent = new Dictionary<int, DashBoardApprRejectCountEvent>();                             // 대쉬보드 조회 카운트 이벤트.

        public Dictionary<int, PasswdChgDayEvent> DicPasswdChgDayEvent = new Dictionary<int, PasswdChgDayEvent>();                                        // 패스워드 변경 날짜 조회 결과 노티.

        public BoardNotiSearchEvent boardSearchEvent;                                                                                                           // 공지사항 내용 조회 결과 노티.

        public Dictionary<int, BoardNotiAfterDashBoardEvent> DicBoardNotiAfterDashBoardEvent = new Dictionary<int, BoardNotiAfterDashBoardEvent>();            // 공지사항 내용 조회 후 대쉬보드 화면 갱신 노티
        //public Dictionary<int, BoardNotiAfterTotalBoardEvent> DicBoardNotiAfterTotalBoardEvent = new Dictionary<int, BoardNotiAfterTotalBoardEvent>();           // 공지사항 내용 조회 후 전체 공지사항 보기 화면 갱신 노티
        public BoardNotiAfterTotalBoardEvent BoardNotiAfterTotalBoard;
        public Dictionary<int, AlarmNotiAfterDashBoardEvent> DicAlarmNotiAfterDashBoardEvent = new Dictionary<int, AlarmNotiAfterDashBoardEvent>();               // 알람 노티 수신 후 대쉬보드 화면 갱신 노티
        public NotiAfterTotalMsgEvent NotiAfterTotalEvent;                                                                                     // 노티 수신후  전체 메시지 화면 갱신 노티

        public NotiAfterTotalAlarmEvent notiAfterTotalAlarmEvent;                                                                                        // 노티 수신 후 전체 알람 화면 갱신 노티

        public LoginAfterSGSideBarEvent loginAfterSGSideBar;                                                                                              // 로그인 후 SGSideBar 화면 갱신 노티.

        public LoginAfterSGHeaderUIEvent loginAfterSGHeaderUI;                                                                                              // 로그인 후 SGHeaderUI 화면 갱신 노티.

        public Dictionary<int, PrivacyNotiEvent> DicPrivacyNotifyEvent = new Dictionary<int, PrivacyNotiEvent>(); //개인정보 NOTIFY
        public Dictionary<int, QueryCountEvent> DicQueryCountEvent = new Dictionary<int, QueryCountEvent>();        //쿼리 카운트 함수 모음 딕셔너리
        public Dictionary<int, QueryListEvent> DicQueryListEvent = new Dictionary<int, QueryListEvent>();        //쿼리 카운트 함수 모음 딕셔너리
        public Dictionary<int, QueryDetailEvent> DicQueryDetailEvent = new Dictionary<int, QueryDetailEvent>();     //메일상세 요청 응답 이벤트 
        public Dictionary<int, QueryRecordExistCheckEvent> DicQueryRecordCheckExistEvent = new Dictionary<int, QueryRecordExistCheckEvent>();
        public Dictionary<int, EmailSendCancelEvent> DicEmailSendCancelEvent = new Dictionary<int, EmailSendCancelEvent>(); //이메일 전송 취소 이벤트 

        public Dictionary<int, FileRecvErrInfoEvent> DicFileRecvErrorEvent = new Dictionary<int, FileRecvErrInfoEvent>();                                      // 파일 수신 Error 이벤트 (Server 혹은 NetLib)
        public Dictionary<int, FileForwardEvent> DicFileForwardEvent = new Dictionary<int, FileForwardEvent>();   // 파일 포워딩 수신 이벤트

        public Dictionary<int, GenericNotiType2Event> DicGenericNotiType2Event = new Dictionary<int, GenericNotiType2Event>();   // GenericNotiType2 수신 이벤트

        public SkipFileNotiEvent SkipFileNotiEventFunc = null;   // 예외파일 신청, 결재 Noti ()

        #endregion

        public Dictionary<int, Dictionary<eCmdList, CommonQueryReciveEvent>> _dicQueryReciveEvent = new Dictionary<int, Dictionary<eCmdList, CommonQueryReciveEvent>>();

        public Dictionary<Enums.EnumPageView, PageDataRefreshEvent> _dicPageDataRefreshEvent = new Dictionary<Common.Enums.EnumPageView, PageDataRefreshEvent>();

        public SFMRefreshEvent sfmRefreshEvent = null;

        public NotiUpdatePolicyEvent notiUpdatePolicyEvent = null;

        public FileMimeRecvEvent fileMimeRecvEvent = null;

        public OLEMimeRecvEvent oleMimeRecvEvent = null;

        private Dictionary<int, DeptInfoNotiEvent> _dicDeptInfoEvnet = new Dictionary<int, DeptInfoNotiEvent>();

        public Dictionary<int, ReconnectCountOutEvent> DicReconnectCountEvent = new Dictionary<int, ReconnectCountOutEvent>(); // Reconnect Count Out 횟수이상 발생시

        public Dictionary<int, UrlTypeForwardDataEvent> DicUrlTypeForwardDataEvent = new Dictionary<int, UrlTypeForwardDataEvent>();

        public SendPkiFileEvent PkiFileSendEventFunc = null;   // pki파일전송용

        public SystemEnvQueryNotiEvent SystemEnvEventFunc = null;

        public SGPageEvent()
        {

        }
        ~SGPageEvent()
        {

        }

        public FileMimeRecvEvent GetFileMimeRecvEvent()
        {
            return fileMimeRecvEvent;
        }

        public void SetFileMimeRecvEvent(FileMimeRecvEvent e)
        {
            fileMimeRecvEvent = e;
        }

        /// <summary> Db 쿼리 조회 결과가 오면, SGHeaderUI에 전달하여, OLE 마임 리스트 세팅</summary>
        public OLEMimeRecvEvent GetOLEMimeRecvEvent() => oleMimeRecvEvent;
        
        /// <summary> Db 쿼리 조회 결과가 오면, SGHeaderUI에 전달하여, OLE 마임 리스트 세팅</summary>
        public void SetOLEMimeRecvEvent(OLEMimeRecvEvent e) => oleMimeRecvEvent = e;

        public SFMRefreshEvent GetSFMRefreshEvent()
        {
            return sfmRefreshEvent;
        }

        public void SetSFMRefreshEvent(SFMRefreshEvent e)
        {
            sfmRefreshEvent = e;
        }

        public NotiUpdatePolicyEvent GetUpdatePolicyEvent()
        {
            return notiUpdatePolicyEvent;
        }

        public void SetUpdatePolicyEvent(NotiUpdatePolicyEvent e)
        {
            notiUpdatePolicyEvent = e;
        }

        public PageDataRefreshEvent GetPageDataRefreshEvent(Enums.EnumPageView ePageView)
        {
            if (_dicPageDataRefreshEvent.ContainsKey(ePageView))
            {
                return _dicPageDataRefreshEvent[ePageView];
            }

            return null;
        }

        public void SetPageDataRefreshEvent(Enums.EnumPageView ePageView, PageDataRefreshEvent pageDataRefreshEvent)
        {
            if (_dicPageDataRefreshEvent.ContainsKey(ePageView))
            {
                _dicPageDataRefreshEvent[ePageView] = pageDataRefreshEvent;
            }
            else
            {
                _dicPageDataRefreshEvent.Add(ePageView, pageDataRefreshEvent);
            }
        }

        public CommonQueryReciveEvent GetQueryReciveEvent(int groupId, eCmdList eCmd)
        {
            if (_dicQueryReciveEvent.ContainsKey(groupId))
            {
                Dictionary<eCmdList, CommonQueryReciveEvent> value = _dicQueryReciveEvent[groupId];
                if (value.ContainsKey(eCmd))
                {
                    return value[eCmd];
                }
            }

            return null;
        }

        public void SetQueryReciveEvent(int groupId, eCmdList eCmd, CommonQueryReciveEvent commonQueryReciveEvent)
        {
            if (_dicQueryReciveEvent.ContainsKey(groupId))
            {
                _dicQueryReciveEvent[groupId][eCmd] = commonQueryReciveEvent;
            }
            else
            {
                Dictionary<eCmdList, CommonQueryReciveEvent> value = new Dictionary<eCmdList, CommonQueryReciveEvent>();
                value.Add(eCmd, commonQueryReciveEvent);
                _dicQueryReciveEvent.Add(groupId, value);
            }
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

        public void SetSvrGPKIEventAdd(int groupid, SvrGPKIEvent e)
        {
            DicSvrGPKIEvent[groupid] = e;
        }
        public SvrGPKIEvent GetSvrGPKIEvent(int groupid)
        {
            SvrGPKIEvent e = null;
            if (DicSvrGPKIEvent.TryGetValue(groupid, out e) == true)
                e = DicSvrGPKIEvent[groupid];
            return e;
        }

        public void SetSvrGPKIRandomEventAdd(int groupid, SvrGPKIRandomKeyEvent e)
        {
            DicSvrGPKIRandomKeyEvent[groupid] = e;
        }

        public SvrGPKIRandomKeyEvent GetSvrGPKIRandomEvent(int groupid)
        {
            SvrGPKIRandomKeyEvent e = null;
            if (DicSvrGPKIRandomKeyEvent.TryGetValue(groupid, out e) == true)
                e = DicSvrGPKIRandomKeyEvent[groupid];
            return e;
        }

        public void SetSvrGPKICertEventAdd(int groupid, SvrGPKICertEvent e)
        {
            DicSvrGPKICertEvent[groupid] = e;
        }

        public SvrGPKICertEvent GetSvrGPKICertEvent(int groupid)
        {
            SvrGPKICertEvent e = null;
            if (DicSvrGPKICertEvent.TryGetValue(groupid, out e) == true)
                e = DicSvrGPKICertEvent[groupid];
            return e;
        }

        public void SetSvrGPKIRegEventAdd(int groupid, SvrGPKIRegEvent e)
        {
            DicSvrGPKIRegEvent[groupid] = e;
        }

        public SvrGPKIRegEvent GetSvrGPKIRegEvent(int groupid)
        {
            SvrGPKIRegEvent e = null;
            if (DicSvrGPKIRegEvent.TryGetValue(groupid, out e) == true)
                e = DicSvrGPKIRegEvent[groupid];
            return e;
        }
        //세션중복 이벤트 설정
        public void SetSessionDuplicateEventAdd(int groupid, SessionDuplicateEvent e)
        {
            DicSessionDuplicateEvent[groupid] = e;
        }
        public SessionDuplicateEvent GetSessionDuplicateEvent(int groupid)
        {
            SessionDuplicateEvent e = null;
            if (DicSessionDuplicateEvent.TryGetValue(groupid, out e) == true)
                e = DicSessionDuplicateEvent[groupid];
            return e;
        }

        public void SetLoginEventAdd(int groupid, LoginEvent e)
        {
            DicLoginEvent[groupid] = e;
        }
        public LoginEvent GetLoginEvent(int groupid)
        {
            LoginEvent e = null;
            if (DicLoginEvent.TryGetValue(groupid, out e) == true)
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
        public void SetEmailApprBatchEvent(int groupid, ResponseEvent e)
        {
            DicEmailApprBatchEvent[groupid] = e;
        }
        public ResponseEvent GetEmailApprBatchEvent(int groupid)
        {
            ResponseEvent e = null;
            if (DicEmailApprBatchEvent.TryGetValue(groupid, out e) == true)
                e = DicEmailApprBatchEvent[groupid];
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
        public void SetQueryCountEvent(int groupid, QueryCountEvent e)
        {
            DicQueryCountEvent[groupid] = e;
        }
        public QueryCountEvent GetQueryCountEvent(int groupid)
        {
            QueryCountEvent e = null;
            if (DicQueryCountEvent.TryGetValue(groupid, out e) == true)
                e = DicQueryCountEvent[groupid];
            return e;
        }
        public void SetQueryListEvent(int groupid, QueryListEvent e)
        {
            DicQueryListEvent[groupid] = e;
        }
        public QueryListEvent GetQueryListEvent(int groupid)
        {
            QueryListEvent e = null;
            if (DicQueryListEvent.TryGetValue(groupid, out e) == true)
                e = DicQueryListEvent[groupid];
            return e;
        }
        public void SetQueryDetailEvent(int groupid, QueryDetailEvent e)
        {
            DicQueryDetailEvent[groupid] = e;
        }
        public QueryDetailEvent GetQueryDetailEvent(int groupid)
        {
            QueryDetailEvent e = null;
            if (DicQueryDetailEvent.TryGetValue(groupid, out e) == true)
                e = DicQueryDetailEvent[groupid];
            return e;
        }
        public void SetQueryRecordExistCheckEvent(int groupid, QueryRecordExistCheckEvent e)
        {
            DicQueryRecordCheckExistEvent[groupid] = e;
        }
        public QueryRecordExistCheckEvent GetQueryRecordExistCheckEvent(int groupid)
        {
            QueryRecordExistCheckEvent e = null;
            if (DicQueryRecordCheckExistEvent.TryGetValue(groupid, out e) == true)
                e = DicQueryRecordCheckExistEvent[groupid];
            return e;
        }
        public void SetEmailSendCancelEvent(int groupid, EmailSendCancelEvent e)
        {
            DicEmailSendCancelEvent[groupid] = e;
        }
        public EmailSendCancelEvent GetEmailSendCancelEvent(int groupid)
        {
            EmailSendCancelEvent e = null;
            if (DicEmailSendCancelEvent.TryGetValue(groupid, out e) == true)
                e = DicEmailSendCancelEvent[groupid];
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
        public DownloadCountEvent GetDownloadCountEvent(int groupid)
        {
            DownloadCountEvent e = null;
            if (DicDownloadCountEvent.TryGetValue(groupid, out e) == true)
                e = DicDownloadCountEvent[groupid];
            return e;
        }
        public void SetDetailSearchEventAdd(int groupid, DetailSearchEvent e)
        {
            DetailSearchEvent temp = null;
            if (DicDetailSearchEvent.TryGetValue(groupid, out temp))
                DicDetailSearchEvent.Remove(groupid);
            DicDetailSearchEvent[groupid] = e;
        }
        public void SetDownloadCountEventAdd(int groupid, DownloadCountEvent e)
        {
            DownloadCountEvent temp = null;
            if (DicDownloadCountEvent.TryGetValue(groupid, out temp))
                DicDownloadCountEvent.Remove(groupid);
            DicDownloadCountEvent[groupid] = e;
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

        public void SetPrivacyNotiEventAdd(int groupid, PrivacyNotiEvent e)
        {
            PrivacyNotiEvent temp = null;
            if (DicPrivacyNotifyEvent.TryGetValue(groupid, out temp))
                DicPrivacyNotifyEvent.Remove(groupid);
            DicPrivacyNotifyEvent[groupid] = e;
        }
        public PrivacyNotiEvent GetPrivacyNotiEvent(int groupid)
        {
            PrivacyNotiEvent e = null;
            if (DicPrivacyNotifyEvent.TryGetValue(groupid, out e) == true)
                e = DicPrivacyNotifyEvent[groupid];
            return e;
        }
        public void SetProxySearchEvent(int groupid, ProxySearchEvent e)
        {
            ProxySearchEvent temp = null;
            if (DicProxySearch.TryGetValue(groupid, out temp))
                DicProxySearch.Remove(groupid);
            DicProxySearch[groupid] = e;
        }
        public void SetCommonResultEvent(int groupid, CommonResultEvent e)
        {
            CommonResultEvent temp = null;
            if (DicCommonResult.TryGetValue(groupid, out temp))
                DicCommonResult.Remove(groupid);
            DicCommonResult[groupid] = e;
        }

        public DeptApprLineReflashEvent GetDeptApprLineReflashEvent(int groupid)
        {
            DeptApprLineReflashEvent e = null;
            if (DicDeptApprLineReflashEvent.TryGetValue(groupid, out e) == true)
                e = DicDeptApprLineReflashEvent[groupid];
            return e;
        }

        public void SetDeptApprLineReflashEventAdd(int groupid, DeptApprLineReflashEvent e)
        {
            DeptApprLineReflashEvent temp = null;
            if (DicDeptApprLineReflashEvent.TryGetValue(groupid, out temp))
                DicDeptApprLineReflashEvent.Remove(groupid);
            DicDeptApprLineReflashEvent[groupid] = e;
        }

        public void SetSecurityApproverSearchEvent(int groupid, SecurityApproverSearchEvent e)
        {
            SecurityApproverSearchEvent temp = null;
            if (DicSecurityApproverEvent.TryGetValue(groupid, out temp))
                DicSecurityApproverEvent.Remove(groupid);
            DicSecurityApproverEvent[groupid] = e;
        }
        public SecurityApproverSearchEvent GetSecurityApproverSearchEvent(int groupid)
        {
            SecurityApproverSearchEvent e = null;
            if (DicSecurityApproverEvent.TryGetValue(groupid, out e) == true)
                e = DicSecurityApproverEvent[groupid];
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
        public void SetFileRecvProgressEventAdd(FileRecvProgressEvent e)
        {
            fileRecvProgressEvent = e;
        }

        public FileRecvProgressEvent GetFileRecvProgressEvent()
        {
            return fileRecvProgressEvent;
        }
        public void ReSetFileRecvProgressEventAdd()
        {
            fileRecvProgressEvent = fileRecvProgressMasterEvent;
        }

        public void SetFileRecvProgressMasterEventAdd(FileRecvProgressEvent e)
        {
            fileRecvProgressMasterEvent = e;
            fileRecvProgressEvent = e;
        }
        public FileRecvProgressEvent GetFileRecvProgressMasterEvent()
        {
            return fileRecvProgressMasterEvent;
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

        public void SetServerRecvUrlEventAdd(int groupid, RecvUrlEvent e)
        {
            RecvUrlEvent temp = null;
            if (DicServerRecvUrlEvent.TryGetValue(groupid, out temp))
                DicServerRecvUrlEvent.Remove(groupid);
            DicServerRecvUrlEvent[groupid] = e;
        }
        public RecvUrlEvent GetServerRecvUrlEvent(int groupid)
        {
            RecvUrlEvent e = null;
            if (DicServerRecvUrlEvent.TryGetValue(groupid, out e) == true)
                e = DicServerRecvUrlEvent[groupid];
            return e;
        }

        public void SetBrowserRecvUrlEventAdd(int groupid, RecvUrlEvent e)
        {
            RecvUrlEvent temp = null;
            if (DicBrowserRecvUrlEvent.TryGetValue(groupid, out temp))
                DicBrowserRecvUrlEvent.Remove(groupid);
            DicBrowserRecvUrlEvent[groupid] = e;
        }
        public RecvUrlEvent GetBrowserRecvUrlEvent(int groupid)
        {
            RecvUrlEvent e = null;
            if (DicBrowserRecvUrlEvent.TryGetValue(groupid, out e) == true)
                e = DicBrowserRecvUrlEvent[groupid];
            return e;
        }

        public void SetServerURLlistEventAdd(int groupid, UrlListEvent e)
        {
            UrlListEvent temp = null;
            if (DicUrlListEvent.TryGetValue(groupid, out temp))
                DicUrlListEvent.Remove(groupid);
            DicUrlListEvent[groupid] = e;
        }
        public UrlListEvent GetServerURLlistEvent(int groupid)
        {
            UrlListEvent e = null;
            if (DicUrlListEvent.TryGetValue(groupid, out e) == true)
                e = DicUrlListEvent[groupid];
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


        public void SetAPTAndVirusNotiDBInsertEventAdd(APTAndVirusNotiDBInsert e)
        {
            AptAndVirusDBInsertEvent = e;
        }
        public APTAndVirusNotiDBInsert GetAPTAndVirusNotiDBInsertEvent()
        {
            return AptAndVirusDBInsertEvent;
        }

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

        public void SetUrlRedirectionSetEventAdd(string strGroupidMenu, UrlRedirectionSettingNotiEvent e)
        {
            UrlRedirectionSettingNotiEvent temp = null;

            if (DicUrlRedirectionSetEvent.TryGetValue(strGroupidMenu, out temp))
                DicUrlRedirectionSetEvent.Remove(strGroupidMenu);
            DicUrlRedirectionSetEvent[strGroupidMenu] = e;
        }
        public UrlRedirectionSettingNotiEvent GetUrlRedirectionSetEvent(string strGroupidMenu)
        {
            UrlRedirectionSettingNotiEvent e = null;
            if (DicUrlRedirectionSetEvent.TryGetValue(strGroupidMenu, out e) == true)
                e = DicUrlRedirectionSetEvent[strGroupidMenu];
            return e;
        }
        public Dictionary<string, UrlRedirectionSettingNotiEvent> GetUrlRedirectionSetEventAll()
        {
            return DicUrlRedirectionSetEvent;
        }

        public void SetUrlRedirectUserPolicyEventAdd(int nGroupID, UrlRedirectionPolicySetNotiEvent e)
        {
            UrlRedirectionPolicySetNotiEvent temp = null;

            if (DicUrlRedirectionUserPolicyEvent.TryGetValue(nGroupID, out temp))
                DicUrlRedirectionUserPolicyEvent.Remove(nGroupID);
            DicUrlRedirectionUserPolicyEvent[nGroupID] = e;
        }
        public UrlRedirectionPolicySetNotiEvent GetUrlRedirectUserPolicyEvent(int nGroupID)
        {
            UrlRedirectionPolicySetNotiEvent e = null;
            if (DicUrlRedirectionUserPolicyEvent.TryGetValue(nGroupID, out e) == true)
                e = DicUrlRedirectionUserPolicyEvent[nGroupID];
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

        /**
        *@brief 화면잠금 시간 변경 
        */
        public ScreenTimeChangeNotiEvent GetScreenTimeChangeNotiEvent()
        {
            return ScrLockTimeChgEvent;
        }

        /**
        *@brief 
        */
        public void SetScreenTimeChangeNotiEvent(ScreenTimeChangeNotiEvent screenTimeChgNoti)
        {
            ScrLockTimeChgEvent = screenTimeChgNoti;
        }

        /**
        *@brief 
        */
        public ScreenTimeInitNotiEvent GetScreenTimeInitNotiEvent()
        {
            return ScrLockTimeInitEvent;
        }

        /**
        *@brief 
        */
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

        public CtrlSideRefreshEvent GetCtrlSideRefreshEvent()
        {
            return ctrlSideRefreshEvent;
        }

        public void SetCtrlSideRefreshEvent(CtrlSideRefreshEvent ctrlSideNoti)
        {
            ctrlSideRefreshEvent = ctrlSideNoti;
        }

        public void SetUrlTypeForwardData(int groupid, UrlTypeForwardDataEvent e)
        {
            UrlTypeForwardDataEvent temp = null;
            if (DicUrlTypeForwardDataEvent.TryGetValue(groupid, out temp))
                DicUrlTypeForwardDataEvent.Remove(groupid);
            DicUrlTypeForwardDataEvent[groupid] = e;
        }

        public UrlTypeForwardDataEvent GetUrlTypeForwardData(int groupid)
        {
            UrlTypeForwardDataEvent e = null;
            if (DicUrlTypeForwardDataEvent.TryGetValue(groupid, out e) == true)
                e = DicUrlTypeForwardDataEvent[groupid];
            return e;
        }


        /// <summary>
        /// 노티로 Update 버전 확인 발생
        /// </summary>
        /// <returns></returns>
        public ClientUpgradeEvent GetClientUpgradeNotiEvent()
        {
            return ClientUpdate;
        }
        /// <summary>
        /// 노티로 Update 버전 확인 발생
        /// </summary>
        /// <param name="updateNoti"></param>
        public void SetClientUpgradeNotiEvent(ClientUpgradeEvent updateNoti)
        {
            ClientUpdate = updateNoti;
        }
        /// <summary>
        /// 첫 화면에서 BIND의 패치버전 비교 시 발생
        /// <br>Update Popup에서 처리 시 발생</br>
        /// <br> 목적지 : SGCtrkSideUI 쪽 Update 버전 확인</br>
        /// </summary>
        /// <returns></returns>
        public ClientUpgradeExeEvent GetClientUpgradeExeNotiEvent()
        {
            return ClientUpgreadeExe;
        }
        /// <summary>
        /// 첫 화면에서 BIND의 패치버전 비교 시 발생
        /// <br>Update Popup에서 처리 시 발생</br>
        /// <br> 목적지 : SGCtrkSideUI 쪽 Update 버전 확인</br>
        /// </summary>
        /// <returns></returns>
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
        public void SetBoardNotiSearchEventAdd(BoardNotiSearchEvent e)
        {
            boardSearchEvent = e;
        }
        public BoardNotiSearchEvent GetBoardNotiSearchEvent()
        {
            return boardSearchEvent;
        }
        public void SetBoardNotiAfterDashBoardEventAdd(int groupid, BoardNotiAfterDashBoardEvent e)
        {
            BoardNotiAfterDashBoardEvent temp = null;
            if (DicBoardNotiAfterDashBoardEvent.TryGetValue(groupid, out temp))
                DicBoardNotiAfterDashBoardEvent.Remove(groupid);
            DicBoardNotiAfterDashBoardEvent[groupid] = e;
        }
        public BoardNotiAfterDashBoardEvent GetBoardNotiAfterDashBoardEvent(int groupid)
        {
            BoardNotiAfterDashBoardEvent e = null;
            if (DicBoardNotiAfterDashBoardEvent.TryGetValue(groupid, out e) == true)
                e = DicBoardNotiAfterDashBoardEvent[groupid];
            return e;
        }
        public void SetBoardNotiAfterTotalBoardEventAdd(BoardNotiAfterTotalBoardEvent e)
        {
            BoardNotiAfterTotalBoard = e;
        }
        public BoardNotiAfterTotalBoardEvent GetBoardNotiAfterTotalBoardEvent()
        {
            return BoardNotiAfterTotalBoard;
        }

        public void SetAlarmNotiAfterDashBoardEventAdd(int groupid, AlarmNotiAfterDashBoardEvent e)
        {
            AlarmNotiAfterDashBoardEvent temp = null;
            if (DicAlarmNotiAfterDashBoardEvent.TryGetValue(groupid, out temp))
                DicAlarmNotiAfterDashBoardEvent.Remove(groupid);
            DicAlarmNotiAfterDashBoardEvent[groupid] = e;
        }
        public AlarmNotiAfterDashBoardEvent GetAlarmNotiAfterDashBoardEvent(int groupid)
        {
            AlarmNotiAfterDashBoardEvent e = null;
            if (DicAlarmNotiAfterDashBoardEvent.TryGetValue(groupid, out e) == true)
                e = DicAlarmNotiAfterDashBoardEvent[groupid];
            return e;
        }

        public void SetNotiAfterTotalMsgEventAdd(NotiAfterTotalMsgEvent e)
        {
            NotiAfterTotalEvent = e;
        }
        public NotiAfterTotalMsgEvent GetNotiAfterTotalMsgEvent()
        {
            return NotiAfterTotalEvent;
        }
        public void SetNotiAfterTotalAlarmEventAdd(NotiAfterTotalAlarmEvent e)
        {
            notiAfterTotalAlarmEvent = e;
        }
        public NotiAfterTotalAlarmEvent GetNotiAfterTotalAlarmEvent()
        {
            return notiAfterTotalAlarmEvent;
        }

        public void SetLoginAfterSGSideBarEventAdd(LoginAfterSGSideBarEvent e)
        {
            loginAfterSGSideBar = e;
        }
        public LoginAfterSGSideBarEvent GetLoginAfterSGSideBarEvent()
        {
            return loginAfterSGSideBar;
        }
        public void SetLoginAfterSGHeaderUIEventAdd(LoginAfterSGHeaderUIEvent e)
        {
            loginAfterSGHeaderUI = e;
        }
        public LoginAfterSGHeaderUIEvent GetLoginAfterSGHeaderUIEvent()
        {
            return loginAfterSGHeaderUI;
        }
        public void SetAddFIleRecvErrEventAdd(int groupid, FileRecvErrInfoEvent e)
        {
            FileRecvErrInfoEvent temp = null;
            if (DicFileRecvErrorEvent.TryGetValue(groupid, out temp))
                DicFileRecvErrorEvent.Remove(groupid);
            DicFileRecvErrorEvent[groupid] = e;
        }
        public FileRecvErrInfoEvent GetAddFIleRecvErrEvent(int groupid)
        {
            FileRecvErrInfoEvent e = null;
            if (DicFileRecvErrorEvent.TryGetValue(groupid, out e) == true)
                e = DicFileRecvErrorEvent[groupid];
            return e;
        }
        public void SetFileForwardNotifyEventAdd(int groupid, FileForwardEvent e)
        {
            FileForwardEvent temp = null;
            if (DicFileForwardEvent.TryGetValue(groupid, out temp))
                DicFileForwardEvent.Remove(groupid);
            DicFileForwardEvent[groupid] = e;
        }
        public FileForwardEvent GetFileForwardNotifyEventAdd(int groupid)
        {
            FileForwardEvent e = null;
            if (DicFileForwardEvent.TryGetValue(groupid, out e) == true)
                e = DicFileForwardEvent[groupid];
            return e;
        }

        public void SetGenericNotiType2EventAdd(int groupid, GenericNotiType2Event e)
        {
            GenericNotiType2Event temp = null;
            if (DicGenericNotiType2Event.TryGetValue(groupid, out temp))
                DicGenericNotiType2Event.Remove(groupid);
            DicGenericNotiType2Event[groupid] = e;
        }
        public GenericNotiType2Event GetGenericNotiType2EventAdd(int groupid)
        {
            GenericNotiType2Event e = null;
            if (DicGenericNotiType2Event.TryGetValue(groupid, out e) == true)
                e = DicGenericNotiType2Event[groupid];
            return e;
        }

        public void SetSkipFileNotiEventAdd(SkipFileNotiEvent e)
        {
            SkipFileNotiEventFunc = e;
        }
        public SkipFileNotiEvent GetSkipFileNotiEventAdd()
        {
            return SkipFileNotiEventFunc;
        }


        public void SetDeptInfoEventAdd(int groupId, DeptInfoNotiEvent e)
        {
            DeptInfoNotiEvent temp = null;
            if (_dicDeptInfoEvnet.TryGetValue(groupId, out temp))
                _dicDeptInfoEvnet.Remove(groupId);
            _dicDeptInfoEvnet[groupId] = e;
        }

        public DeptInfoNotiEvent GetDeptInfoEvent(int groupId)
        {
            DeptInfoNotiEvent e = null;
            if (_dicDeptInfoEvnet.TryGetValue(groupId, out e))
                e = _dicDeptInfoEvnet[groupId];

            return e;
        }

        public void SetReconnectCountOutEventAdd(int groupid, ReconnectCountOutEvent e)
        {
            ReconnectCountOutEvent temp = null;
            if (DicReconnectCountEvent.TryGetValue(groupid, out temp))
                DicReconnectCountEvent.Remove(groupid);
            DicReconnectCountEvent[groupid] = e;
        }

        public ReconnectCountOutEvent GetReconnectCountOutEvent(int groupid)
        {
            ReconnectCountOutEvent e = null;
            if (DicReconnectCountEvent.TryGetValue(groupid, out e) == true)
                e = DicReconnectCountEvent[groupid];
            return e;
        }

        public void SetPkiFileSendEventAdd(SendPkiFileEvent e)
        {
            PkiFileSendEventFunc = e;
        }
        public SendPkiFileEvent GetPkiFileSendEvent()
        {
            return PkiFileSendEventFunc;
        }

        public SystemEnvQueryNotiEvent GetSystemEnvQueryResultEvent()
        {
            return SystemEnvEventFunc;
        }
        public void SetSystemEnvQueryResultEvent(SystemEnvQueryNotiEvent SystemEnvNoti)
        {
            SystemEnvEventFunc = SystemEnvNoti;
        }

    }
}
