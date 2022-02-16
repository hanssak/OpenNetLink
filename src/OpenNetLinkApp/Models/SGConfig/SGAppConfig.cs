using Radzen.Blazor.Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Serilog;
using Serilog.Events;
using AgLogManager;

namespace OpenNetLinkApp.Models.SGConfig
{
    public class SGAppConfig : ISGAppConfig
    {
        public List<string> ClipBoardHotKey { get; set; } = null;                           // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet).

        //public List<string> ClipBoardHotKeyNetOver { get; set; } = null;                  // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet,3중망Idx(2이상존재)).
        public Dictionary<string, string> ClipBoardHotKeyNetOver { get; set; } = null;      // 클립보드 단축키 정보 ( <nGroupID-Idx, "Win,Ctrl,Alt,Shift,Alphabet"> 3중망Idx(2이상존재) )
        public CLIPALM_TYPE enClipAlarmType { get; set; } = CLIPALM_TYPE.OSUI;              // 클립보드 알림 형식  ( 0 : OS & UI , 1 : OS, 2 : UI )
        public PAGE_TYPE enMainPageType { get; set; } = PAGE_TYPE.NONE;                     // 메인페이지 리스트 (0 : DASHBOARD, 1 : TRANSFER)
        public bool bClipAfterSend { get; set; } = false;                                   // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        //public bool bURLAutoTrans { get; set; } = false;                                  // URL 자동전환 사용 유무 ( true : 사용, false : 미사용 )
        public List<bool> bURLAutoTrans { get; set; } = null;                               // URL 자동전환 사용 유무 (망별로) ( true : 사용, false : 미사용 )
        public List<bool> bURLAutoAfterMsg { get; set; } = null;                            // URL 자동전환 후 사용자 알림 메시지 사용 여부(망별로) ( true : 사용, false : 미사용 )
        public List<string> strURLAutoAfterBrowser { get; set; } = null;                    // URL 자동전환 후 브라우저 창 처리방식(망별로) ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        public List<string> strForwardUrl { get; set; } = null;                             // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )
        public bool bRMouseFileAddAfterTrans { get; set; } = false;                         // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        public bool bAfterBasicChk { get; set; } = false;                                   // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        public List<string> RecvDownPath { get; set; } = null;                              // 파일 수신 경로 정보
        public bool bFileRecvFolderOpen { get; set; } = true;                               // 파일 수신 후 폴더 자동 열기 ( true : 열기, flase : 열지 않음 )
        public bool bRecvDownPathChange { get; set; } = true;                               // 파일 수신 경로 변경 가능 여부 ( true : 가능, false : 불가능 )
        public bool bManualRecvDownChange { get; set; } = false;                            // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        public bool bFileRecvTrayFix { get; set; } = false;                                 // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprTrayFix { get; set; } = false;                                     // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprActionTrayFix { get; set; } = false;                           // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprRejectTrayFix { get; set; } = false;                           // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bExitTrayMove { get; set; } = false;                                    // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartTrayMove { get; set; } = false;                                   // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartProgramReg { get; set; } = false;                                 // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )
        public string strLanguage { get; set; } = "KR";                                     // 다국어 지원 ( KR : 한국어, JP : 일본어, EN : 영어, CN : 중국어 )

        //public bool bUseScreenLock { get; set; } = true;                                  // 화면잠금 사용 여부
        public bool bScreenLock { get; set; } = true;                                       // 화면잠금 사용 여부.(체크)
        public bool bScreenLockUserChange { get; set; } = false;                            //스크린 잠금 사용자 임의 변경 가능여부
        public int  tScreenTime { get; set; } = 5;                                          // 화면잠금 시간 설정( 단위 : 분 )       
        public LogEventLevel   LogLevel { get; set; } = LogEventLevel.Information;          // 로그레벨
        public bool bUseApprWaitNoti { get; set; } = true;                                  // 승인대기 알림 사용 여부.(체크)
        public string UpdateSvcIP { get; set; } = string.Empty;                             // 업데이트 서버 IP
        public bool bUseLogLevel { get; set; } = false;                                     // 로그 레벨 사용 여부
        public List<bool> listUseGpkiLogin { get; set; } = null;                            // GPKI 로그인 사용 여부
        public bool bUseOverNetwork2 { get; set; } = false;                                 // 3망 전송 사용 유무
        public bool bUseNetOverAllsend { get; set; } = false;
        public bool bFileForward { get; set; } = false;                             //파일포워드 사용유무
        public bool bEmailApproveUse { get; set; } = false;                         //이메일결재 사용유무
        public bool bClipboardApproveUse { get; set; } = false;                     //클립보드 결재 사용유무
        public bool bShowAdminInfo { get; set; } = false;                           //대시보드에 관리자 정보 표시 여부 
        public bool bUseFileCheckException { get; set; } = false;                   //파일검사 예외 신청
        public bool bDenyPasswordZIP { get; set; } = false;                         //패스워드걸린 ZIP파일 전송허용여부

    }

}
