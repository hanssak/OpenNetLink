using System;
using System.Collections.Generic;
using System.Text;

using Serilog;
using Serilog.Events;
using AgLogManager;
using System.ComponentModel;

namespace OpenNetLinkApp.Models.SGConfig

{
    public enum CLIPALM_TYPE : int
    {
        [Description("OS & UI")]
        OSUI = 0,
        [Description("OS")]
        OS = 1,
        [Description("UI")]
        UI = 2,
    }

    // MoveTo. ISGopConfig.cs
    public enum PAGE_TYPE : int
    {
        [Description("초기값")]
        NONE = 0,
        [Description("대시보드")]
        DASHBOARD = 1,
        [Description("파일전송")]
        TRANSFER = 2
    }
    public enum HOTKEY_MOD: int
    {
        // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
        WINDOW = 0,
        CTRL = 1,
        ALT = 2,
        SHIFT = 3,
        VKEY = 4,
        NETOVER_IDX = 5
    }

    public interface ISGAppConfig
    {
	
		// Used (AppEnvSetting.json)
        List<string> ClipBoardHotKey { get; }                       // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
        Dictionary<string, string> ClipBoardHotKeyNetOver { get; }  // 클립보드 단축키 정보 ( <nGroupID-Idx, "Win,Ctrl,Alt,Shift,Alphabet"> 3중망Idx(2이상존재) )
        CLIPALM_TYPE enClipAlarmType { get; }                       // 클립보드 알림 형식  ( 0 : OS & UI , 1 : OS, 2 : UI )
        LogEventLevel LogLevel { get; }                             // 로그레벨
        List<string> RecvDownPath { get; }                          // 파일 수신 경로 정보
        string UpdateSvcIP { get; }                                 // 업데이트 서버 IP
        bool bFileRecvFolderOpen { get; }                           // 파일 수신 후 폴더 자동 열기 ( true : 열기, flase : 열지 않음 )
        string strLanguage { get; }                                 // 다국어 지원 ( KR : 한국어, JP : 일본어, EN : 영어, CN : 중국어 )
        int  tScreenTime { get; }                                   // 화면잠금 시간 설정( 단위 : 분 )
		int nUserSelectFirstNet { get; }

        // notUse (MoveTo SGopConfig.json)
		// MoveTo. ISGopConfig.cs
        List<bool> bURLAutoTrans { get; }                           // URL 자동전환 사용 유무(groupID별로) ( true : 사용, false : 미사용 )
        List<bool> bURLAutoAfterMsg { get; }                        // URL 자동전환 후 사용자 알림 메시지 사용 여부(groupID별로) ( true : 사용, false : 미사용 )		
        List<string> strURLAutoAfterBrowser { get; }                // URL 자동전환 후 브라우저 창 처리방식 ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        bool bRMouseFileAddAfterTrans { get; }                      // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        bool bAfterBasicChk { get; }                                // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        //bool bManualRecvDownChange { get; }                         // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
		
		
        bool bFileRecvTrayFix { get; }                              // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bApprTrayFix { get; }                                  // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bUserApprActionTrayFix { get; }                        // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bUserApprRejectTrayFix { get; }                        // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bExitTrayMove { get; }                                 // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        bool bStartTrayMove { get; }                                // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        bool bStartProgramReg { get; }                              // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )

        //bool bUseScreenLock { get; }                              // 화면잠금 사용 여부.
        bool bScreenLock { get; }                                   // 화면잠금 사용 여부.(체크)
        bool bUseApprWaitNoti { get; }                              // 승인대기 알림 사용 여부.(체크)
        //public bool bUseLogLevel { get; set; }                      // 로그 레벨 사용 여부
        //public List<bool> listUseGpkiLogin { get; set; }            // GPKI 로그인 사용 여부



        //public bool bScreenLockUserChange { get; set; }             //스크린락 사용자 임의 조작 가능 여부
        //public bool bShowAdminInfo { get; set; }                    //대쉬보드에 관리자 정보 표시여부
        //public bool bUseAppLoginType { get; set; }                  //사용자 지정 로그인타입 사용 여부
        public int LoginType { get; set; }                          //사용자 지정 로그인타입
		
        //public bool bEmailApproveUse { get; set; }                  // 이메일결재 사용유무


		
		PAGE_TYPE enMainPageType { get; }                           // 메인페이지 (0 : NONE, 1 : DASHBOARD,  2 : TRANSFER)				
        //public bool bUseNetOverAllsend { get; set; }                // 3망 전송에서 전체 사용자에게 보내는 기능 사용유무
        List<string> strForwardUrl { get; }                         // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )
        //public bool bFileDownloadBeforeReciving { get; set; }       // 파일포워드 사용시 PC 미수신한 상태에서도 다운로드 가능 유무
        //public bool bUseFileCheckException { get; set; }            // 파일검사 예외신청 사용유무

		// Not Work - 설정값은 있지만 동작 구현 안되어 있음
        bool bClipCopyAutoSend { get; }                                // 클립보드 복사 후 자동전송 기능 사용 유무 ( true : 사용, false : 미사용 )


		
		// notUse (MoveTo SGSiteConfig.cs)
        //public bool bDenyPasswordZIP { get; set; }                  // 패스워드 걸린압축파일 전송허용 여부
        //public bool bFileForward { get; set; }                      // 파일포워드 사용유무		
        //public bool bClipboardFileTransUse { get; set; }            // 클립보드 파일전송 형태로 사용
        //public bool bClipboardManageUse { get; set; }               // 클립보드 관리/결재 UI 나오게 설정

    }
}
