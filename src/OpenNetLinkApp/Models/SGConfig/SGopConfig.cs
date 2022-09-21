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
    public class SGopConfig : ISGopConfig
    {
        public PAGE_TYPE enMainPageType { get; set; } = PAGE_TYPE.NONE;                     // 메인페이지 리스트 (0 : DASHBOARD, 1 : TRANSFER)
        public bool bClipCopyAutoSend { get; set; } = false;                                   // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        //public bool bURLAutoTrans { get; set; } = false;                                  // URL 자동전환 사용 유무 ( true : 사용, false : 미사용 )
        public List<bool> bURLAutoTrans { get; set; } = null;                               // URL 자동전환 사용 유무 (망별로) ( true : 사용, false : 미사용 )
        public List<bool> bURLAutoAfterMsg { get; set; } = null;                            // URL 자동전환 후 사용자 알림 메시지 사용 여부(망별로) ( true : 사용, false : 미사용 )
        public List<string> strURLAutoAfterBrowser { get; set; } = null;                    // URL 자동전환 후 브라우저 창 처리방식(망별로) ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        public List<string> strForwardUrl { get; set; } = null;                             // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )
        public bool bRMouseFileAddAfterTrans { get; set; } = false;                         // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        public bool bAfterBasicChk { get; set; } = false;                                   // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        public bool bRecvDownPathChange { get; set; } = true;                               // 파일 수신 경로 변경 가능 여부 ( true : 가능, false : 불가능 )
        public bool bManualRecvDownChange { get; set; } = false;                            // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        public bool bFileRecvTrayFix { get; set; } = false;                                 // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprTrayFix { get; set; } = false;                                     // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprActionTrayFix { get; set; } = false;                           // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprRejectTrayFix { get; set; } = false;                           // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bExitTrayMove { get; set; } = false;                                    // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartTrayMove { get; set; } = false;                                   // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartProgramReg { get; set; } = false;                                 // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )

        //public bool bUseScreenLock { get; set; } = true;                                  // 화면잠금 사용 여부
        public bool bScreenLock { get; set; } = true;                                       // 화면잠금 사용 여부.(체크)
        public bool bScreenLockUserChange { get; set; } = false;                            //스크린 잠금 사용자 임의 변경 가능여부
        public bool bUseApprWaitNoti { get; set; } = true;                                  // 승인대기 알림 사용 여부.(체크)
        public bool bUseLogLevel { get; set; } = false;                                     // 로그 레벨 사용 여부
        public List<bool> listUseGpkiLogin { get; set; } = null;                            // GPKI 로그인 사용 여부
        public bool bUseNetOverAllsend { get; set; } = false;        
        public bool bFileDownloadBeforeReciving { get; set; } = false;              //파일포워드 사용시 PC 미수신한 상태에서도 다운로드 가능 유무
        

        public bool bShowAdminInfo { get; set; } = false;                           //대시보드에 관리자 정보 표시 여부 

        public bool bUseFileCheckException { get; set; } = false;                   //파일검사 예외 신청
        public bool bUseAppLoginType { get; set; } = false;                         //사용자 지정 로그인타입 사용 여부
        public int LoginType { get; set; } = 0;                                   //사용자 지정 로그인타입 지정

        // Add
        public bool bNoApproveManageUI { get; set; } = false;                             // 결재관리 No사용 유무 ( true : 결재관리UI / NoTi 없어짐, false : 기존설정대로사용 )

        public bool bEmptyfileTrans { get; set; } = false;                                      // 0kb 파일 송수신 가능 유무

        public bool bUseFileExceptionDescCheck { get; set; } = true;                           // 파일 예외신청 설명정보 필수 기업여부

    }
}
