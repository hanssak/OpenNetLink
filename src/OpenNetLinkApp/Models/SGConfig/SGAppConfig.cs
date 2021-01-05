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

        public List<string> ClipBoardHotKeyNetOver { get; set; } = null;                    // 클립보드 단축키 정보 (Win,Ctrl,Alt,Shift,Alphabet,3중망Idx(2이상존재)).

        public CLIPALM_TYPE enClipAlarmType { get; set; } = CLIPALM_TYPE.OSUI;              // 클립보드 알림 형식  ( 0 : OS & UI , 1 : OS, 2 : UI )
        public bool bClipAfterSend { get; set; } = false;                                   // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        public bool bURLAutoTrans { get; set; } = false;                                    // URL 자동전환 사용 유무 ( true : 사용, false : 미사용 )
        public bool bURLAutoAfterMsg { get; set; } = false;                                 // URL 자동전환 후 사용자 알림 메시지 사용 여부( true : 사용, false : 미사용 )
        public string strURLAutoAfterBrowser { get; set; } = "C";                           // URL 자동전환 후 브라우저 창 처리방식 ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
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

        //public bool bUseScreenLock { get; set; } = true;                                    // 화면잠금 사용 여부
        public bool bScreenLock { get; set; } = true;                                       // 화면잠금 사용 여부.(체크)
        public int  tScreenTime { get; set; } = 5;                                          // 화면잠금 시간 설정( 단위 : 분 )
        public string LastUpdated { get; set; } = DateTime.Now.ToString(@"yyyy\/MM\/dd h\:mm tt"); // 마지막으로 업데이트된 날짜/시간정보
        public string SWVersion { get; set; } = "1.0.0.0";                                  // 소프트웨어 버전 정보
        public string SWCommitId { get; set; } = "ad9f269";                                 // 소프트웨어 버전 정보 : Git Commit Point for this Released S/W
        public LogEventLevel   LogLevel { get; set; } = LogEventLevel.Information;          // 로그레벨
        public bool bUseApprWaitNoti { get; set; } = true;                                  // 승인대기 알림 사용 여부.(체크)
        public string UpdateSvcIP { get; set; } = string.Empty;                             // 업데이트 서버 IP
        public string UpdatePlatform { get; set; } = string.Empty;                          // 업데이트 될 OpenNetLinkApp Machine Architecture 플랫폼
        public bool bUseLogLevel { get; set; } = false;                                     // 로그 레벨 사용 여부
        public List<bool> listUseGpkiLogin { get; set; } = null;                            // GPKI 로그인 사용 여부

        public bool bUseOverNetwork2 { get; set; } = false;                                     // 3망 전송 사용 유무
        /*
                public string strClipBoardHotKey { get; set; } = "Y,Y,Y,Y,V";                   // 클립보드 단축키 정보.
                public int strRecvClipAlarm { get; set; } = 0;                                  // 클립보드 알림 형식  ( 0 : OS & UI , 1 : OS, 2 : UI )
                public bool bClipAfterSend { get; set; } = false;                               // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
                public bool bURLAutoTrans { get; set; } = false;                                // URL 자동전환 사용 유무 ( true : 사용, false : 미사용 )
                public string bURLAutoAfterBrowser { get; set; } = "F";                           // URL 자동전환 후 브라우저 창 처리방식 ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
                public bool bURLAutoAfterMsg { get; set; } = false;                             // URL 자동전환 후 사용자 알림 메시지 사용 여부( true : 사용, false : 미사용 )
                public bool bRMouseFileAddAfterTrans { get; set; } = false;                     // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
                public bool bAfterBasicChk { get; set; } = false;                               // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )

                public string strRecvDownPath { get; set; } = System.IO.Directory.GetCurrentDirectory();     // 파일 수신 경로 정보
                public bool bRecvDownPathChange { get; set; } = true;                                       // 파일 수신 경로 변경 가능 여부 ( true : 가능, false : 불가능 )
                public bool bFileRecvFolderOpen { get; set; } = true;                                       // 파일 수신 후 폴더 자동 열기 ( true : 열기, flase : 열지 않음 )
                public bool bManualRecvDownChange { get; set; } = false;                                    // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
                public bool bFileRecvTrayMaintain { get; set; } = false;                                    // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
                public bool bApprTrayMaintain { get; set; } = false;                                        // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
                public bool bUserApprActionTrayMaintain { get; set; } = false;                              // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
                public bool bUserApprRejectTrayMaintain { get; set; } = false;                              // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
                public bool bExitTrayMove { get; set; } = false;                                            // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
                public bool bStartTrayMove { get; set; } = false;                                           // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
                public bool bStartProgramReg { get; set; } = false;                                         // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )
                public string strLanguage { get; set; } = "KR";                                               // 다국어 지원 ( KR : 한국어, JP : 일본어, EN : 영어, CN : 중국어 )

                public bool bScreenLock { get; set; } = false;                                              // 화면잠금 사용 여부.
                public int nScreenTime { get; set; } = 0;                                                   // 화면잠금 시간 설정( 단위 : 분 )
        */

    }

}
