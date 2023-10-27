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
        [Description("T_PAGE_TYPE_INIT")]
        NONE = 0,
        [Description("T_PAGE_TYPE_DASHBOARD")]
        DASHBOARD = 1,
        [Description("T_PAGE_TYPE_TRNASFER")]
        TRANSFER = 2,
        [Description("T_PAGE_TYPE_FILE_TRANSMANAGER")]
        TRANSMANAGER_FILE = 3,
        [Description("T_PAGE_TYPE_CLIP_TRANSMANAGER")]
        TRANSMANAGER_CLIP = 4,
        [Description("T_PAGE_TYPE_EMAIL_TRANSMANAGER")]
        TRANSMANAGER_EMAIL = 5
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

    /// <summary>
    /// OpenNetLink 로그인을 요청하는 NAC 종류
    /// </summary>
    public enum NAC_LOGIN_TYPE : int
    {
        NONE=0,
        Genian =1,
    }

    public interface ISGAppConfig
    {
	
		// Used (AppEnvSetting.json)
        List<string> ClipBoardHotKey { get; }                       // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).
        Dictionary<string, string> ClipBoardHotKeyNetOver { get; }  // 클립보드 단축키 정보 ( <nGroupID-Idx, "Win,Ctrl,Alt,Shift,Alphabet"> 3중망Idx(2이상존재) )
        CLIPALM_TYPE enClipAlarmType { get; }                       // 클립보드 알림 형식  ( 0 : OS & UI , 1 : OS, 2 : UI )
        PAGE_TYPE enMainPageType { get; }                           // 메인페이지 (0 : NONE, 1 : DASHBOARD,  2 : TRANSFER)	
        bool bClipCopyAutoSend { get; }                                // 클립보드 복사 후 자동전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        List<bool> bURLAutoTrans { get; }                           // URL 자동전환 사용 유무(groupID별로) ( true : 사용, false : 미사용 )                                                                                         
        List<bool> bURLAutoAfterMsg { get; }                        // URL 자동전환 후 사용자 알림 메시지 사용 여부(groupID별로) ( true : 사용, false : 미사용 )		
        List<string> strURLAutoAfterBrowser { get; }                // URL 자동전환 후 브라우저 창 처리방식 ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        List<string> strForwardUrl { get; }                         // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )
        bool bRMouseFileAddAfterTrans { get; }                      // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        bool bAfterBasicChk { get; }                                // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        List<string> RecvDownPath { get; }                          // 파일 수신 경로 정보
        bool bFileRecvFolderOpen { get; }                           // 파일 수신 후 폴더 자동 열기 ( true : 열기, flase : 열지 않음 )
        public bool bManualRecvDownChange { get; }                   // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        bool bFileRecvTrayFix { get; }                              // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bApprTrayFix { get; }                                  // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bUserApprActionTrayFix { get; }                        // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bUserApprRejectTrayFix { get; }                        // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bExitTrayMove { get; }                                 // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        bool bStartTrayMove { get; }                                // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        bool bStartProgramReg { get; }                              // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )
        string strLanguage { get; }                                 // 다국어 지원 ( KR : 한국어, JP : 일본어, EN : 영어, CN : 중국어 )
        //bool bUseScreenLock { get; }                                  // 화면잠금 사용 여부.(체크)
        //int  tScreenTime { get; }                                   // 화면잠금 시간 설정( 단위 : 분 )
        public LogEventLevel LogLevel { get; }                      // 로그레벨
        bool bUseApprWaitNoti { get; }                              // 승인대기 알림 사용 여부.(체크)
        int nUserSelectFirstNet { get; }
        List<bool> bAskFileSend { get; }                            //파일리스트 추가 후 전송을 묻는 팝업표시 여부 (템플릿에 한하여  Default : true)
        bool bHideSideBarAfterLogin { get; }                           // 로그인 후 좌측 사이드바 숨김 여부 설정(Default :false)
    }
}
