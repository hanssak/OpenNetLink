using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGConfig

{
    public interface ISGAppConfig
    {
        public string strClipboardHotKey { get; set; }                    // 클립보드 단축키 정보.
        public int strRecvClipAlarm { get; set; }                                  // 클립보드 알림 형식  ( 0 : OS & UI , 1 : OS, 2 : UI )
        public bool bClipAfterSend { get; set; }                               // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        public bool bURLAutoTrans { get; set; }                                 // URL 자동전환 사용 유무 ( true : 사용, false : 미사용 )
        public string bURLAutoAfterBrowser { get; set; }                           // URL 자동전환 후 브라우저 창 처리방식 ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        public bool bURLAutoAfterMsg { get; set; }                            // URL 자동전환 후 사용자 알림 메시지 사용 여부( true : 사용, false : 미사용 )
        public bool bRMouseFileAddAfterTrans { get; set; }                      // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        public bool bAfterBasicChk { get; set; }                                // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )

        public string strRecvDownPath { get; set; }      // 파일 수신 경로 정보
        public bool bRecvDownPathChange { get; set; }                                      // 파일 수신 경로 변경 가능 여부 ( true : 가능, false : 불가능 )
        public bool bFileRecvFolderOpen { get; set; }                                        // 파일 수신 후 폴더 자동 열기 ( true : 열기, flase : 열지 않음 )
        public bool bManualRecvDownChange { get; set; }                                    // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        public bool bFileRecvTrayMaintain { get; set; }                                   // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprTrayMaintain { get; set; }                                        // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprActionTrayMaintain { get; set; }                              // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprRejectTrayMaintain { get; set; }                              // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bExitTrayMove { get; set; }                                             // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartTrayMove { get; set; }                                         // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartProgramReg { get; set; }                                        // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )
        public string strLanguage { get; set; }                                              // 다국어 지원 ( KR : 한국어, JP : 일본어, EN : 영어, CN : 중국어 )

        public bool bScreenLock { get; set; }                                              // 화면잠금 사용 여부.
        public int nScreenTime { get; set; }                                                   // 화면잠금 시간 설정( 단위 : 분 )
    }
}
