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
        public bool bManualRecvDownChange { get; set; } = false;                            // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        public bool bFileRecvTrayFix { get; set; } = false;                                 // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprTrayFix { get; set; } = false;                                     // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprActionTrayFix { get; set; } = false;                           // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUserApprRejectTrayFix { get; set; } = false;                           // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bExitTrayMove { get; set; } = false;                                    // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartTrayMove { get; set; } = false;                                   // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bStartProgramReg { get; set; } = false;                                 // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )

        public bool bScreenLock { get; set; } = false;                                       // 화면잠금 사용 여부.(체크)
        public bool bScreenLockUserChange { get; set; } = false;                            //스크린 잠금 사용자 임의 변경 가능여부
        public bool bUseApprWaitNoti { get; set; } = true;                                  // 승인대기 알림 사용 여부.(체크)
        public bool bUseLogLevel { get; set; } = false;                                     // 로그 레벨 사용 여부
        public List<bool> listUseGpkiLogin { get; set; } = null;                            // GPKI 로그인 사용 여부
        public bool bUseNetOverAllsend { get; set; } = false;
        public bool bFileDownloadBeforeReciving { get; set; } = false;              //파일포워드 사용시 PC 미수신한 상태에서도 다운로드 가능 유무

        public bool bShowAdminInfo { get; set; } = false;                           //대시보드에 관리자 정보 표시 여부 

        public bool bUseFileCheckException { get; set; } = false;                   // 파일검사 예외 신청( Virus / Apt )
        public bool bUseAppLoginType { get; set; } = false;                         //사용자 지정 로그인타입 사용 여부
        public int LoginType { get; set; } = 0;                                   //사용자 지정 로그인타입 지정

        // Add
        public bool bNoApproveManageUI { get; set; } = false;                             // 결재관리 No사용 유무 ( true : 결재관리UI / NoTi 없어짐, false : 기존설정대로사용 )

        public bool bEmptyfileTrans { get; set; } = false;                                      // 0kb 파일 송수신 가능 유무


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // siteConfig Code Level에서 동작하던거 AppOPsetting.json 으로 옮겨 온거

        public bool bUserIDSave { get; set; } = false;                                      // 로그인한 ID 저장 여부

        public bool bAutoLogin { get; set; } = false;                                       // 자동로그인 사용 여부.

        public bool bAutoLoginCheck { get; set; } = false;                                  // 자동로그인 체크박스 체크여부.

        public bool bApprLineLocalSave { get; set; } = false;                               // 결재라인 로컬 저장 여부.

        public bool bTitleDescSameChk { get; set; } = false;                                // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부

        public bool bApprLineChkBlock { get; set; } = true;                                              // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )

        public bool bApprDeptSearch { get; set; } = true;                                   // 결재자 검색 창의 타부서 수정 가능 여부.

        public bool bUserPWChange { get; set; } = false;                   // 사용자 패스워드 변경 사용 여부.

        public string strPWChangeProhibitLimit { get; set; } = "";        // 패스워드 사용금지 문자열 지정.

        public int nPWChangeApplyCnt { get; set; } = 9;                   // 패스워드 변경 시 허용되는 자리수 지정.
        public string strInitPasswd { get; set; } = "1K27SdexltsW0ubSCJgsZw==";    // 초기 패스워드 정보.(hsck@2301)


        public bool bRecvFolderChange { get; set; } = true;               // 수신 폴더 변경 사용 여부.
        public bool bUseUserRecvDownPath { get; set; } = false;           // 로그인 유저별 다운로드 경로 사용 여부
        //public bool bUseEmail { get; set; } = false;               

        //public bool bUseUserRecvDownPathUserName { get; set; } = true;           // 로그인 유저별 다운로드 경로 사용때, 사용자ID가 아니라 사용자이름 사용할지 유무
        

        public List<bool> blistUseEmail { get; set; } = null;               // 메일 관리/결재 사용 유무.

        public List<bool> blistUiDlpShow { get; set; } = null;    // 메일 관리/결재 에서 개인정보 검색항목 View 및 Search 기능 display 유무

        public bool bUsePCURL { get; set; } = false;                      // PCURL 사용 유무.

        public bool bUsePublicBoard { get; set; } = true;                // 공지사항 사용 유무.
        public bool bUseCertSend { get; set; } = false;                   // 인증서 전송 사용 유무.
        //public bool m_bUseOSMaxFilePath { get; set; } = true;               // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 

        /// <summary>
        /// ////////////////////////////////
        /// </summary>

        public bool bUseClipCopyAndSend { get; set; } = false;    // 클립보드 복사 후 전송 사용 유무
        public bool bRFileAutoSend { get; set; } = false;    // 마우스 우클릭 후 자동전송 사용 유무
        public bool bShowAfterApprAutoCheck { get; set; } = false;    // 사후결재 기본 체크 사용유무 : 공통환경설정에 나오도록 할지 유무

        public bool bRecvFolderOpen { get; set; } = false;    // 파일 수신 후 폴더 열기 사용 유무
        public bool bManualDownFolderChange { get; set; } = false;    // 수동다운로드로 다운 시 폴더 선택 사용 유무
        public bool bFileRecvAlarmRetain { get; set; } = false;    // 파일 수신 후 알림 유지 사용 유무
        public bool bApprCountAlarmRetain { get; set; } = false;    // 승인대기 알림 유지 사용 유무
        public bool bApprCompleteAlarmRetain { get; set; } = false;    // 승인완료 알림 유지 사용 유무
        public bool bApprRejectAlarmRetain { get; set; } = false;    // 반려 알림 유지 사용 유무
        public bool bUseApprCountAlaram { get; set; } = false;    // 승인대기 알림 사용 유무.
        public bool bUseCloseTrayMove { get; set; } = false;    // 종료 시 트레이 사용 유무.
        public bool bUseStartTrayMove { get; set; } = false;    // 프로그램 시작 시 트레이 이동 사용 유무.
        public bool bUseStartProgramReg { get; set; } = false;    // 시작 프로그램 등록 사용 유무.
        public bool bUseLanguageSet { get; set; } = false;    // 언어설정 사용 유무.

        public bool bUseMainPageType { get; set; } = true;   // 메인화면 사용 여부

        public bool bViewFileFilter { get; set; } = true;   // (환경설정) 확장자 제한 화면 표시 유무.
        public bool bViewSGSideBarUIBadge { get; set; } = false;   // 왼쪽 메뉴들에서 Badge 나오게할지 유무 설정값
        public bool bViewSGHeaderUIAlarmNoriAllDel { get; set; } = true;   // 상단 HeaderUI에서 Alarm, Noti 상에 Badge 전체 삭제 메뉴 나오게할지 유무
        public bool bUseForceUpdate { get; set; } = true;   // 넘기는 기능 없이 무조건 업데이트 사용 유무
        public bool bUseForceBackgroundUpdate { get; set; } = false; //무조건 업데이트 시 별도 팝업 및 클릭없이 자동 업데이트

        public bool bViewDlpApproverSelectMyDept { get; set; } = false;   // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)
        public bool bClipBoardNoApproveButFileTrans { get; set; } = false;   // 클립보드 파일전송 사용 형태로 결재없이 동작
        public int nClipAfterApproveUseType { get; set; } = 0;   // 클립보드 파일전송형태 전송때, 0:CheckBox 및 결재 설정대로, 1:사전, 2:사후 로 전송되게 적용
        public bool bUseUserSelectFirstServer { get; set; } = false;   // 사용자가 처음접속하는 Server(Network) 를 선택할 수 있을지 유무

        public bool bUseFileForward { get; set; } = false;   // 파일포워드 기능 사용유무


        public bool bUseClipAlarmType { get; set; } = true;    // clipboard 송수신 알림 형태 수정가능유무

        public bool bUseInitAlarmPerDay { get; set; } = false; // 일일 이전 날짜 알림 자동 삭제

        public bool bUseInitMessagePerDay { get; set; } = false; // 일일 이전 날짜 메세지 자동 삭제

        public bool bVisiblePolicyUpdateButton { get; set; } = true; //정책 업데이트 버튼 보여주기
        /// <summary>
        /// ////////////////////////////////
        /// </summary>

        public bool bUseDenyPasswordZip { get; set; } = false;         // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool bUseClipBoardFileTrans { get; set; } = true;         // 파일형태로보내는 클립보드 사용 유무
        public bool bUseFileClipManageUI { get; set; } = true;         // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool bUseFileClipApproveUI { get; set; } = false;          // 파일형태로보내는 클립보드 결재UI 나오게할지 유무

        public bool bUseOneToMultiLogin { get; set; } = false;         // 1번에 다중망 로그인 기능 사용유무
        public bool bUseOneByOneLogOut { get; set; } = false;         // 1번에 다중망 로그인 기능 사용때에도 로그아웃은 선택한 망에서 개별 로그아웃적영

        public bool bUseAgentTime1aClock { get; set; } = false;         // 사후결재 정책, 자정에  검색화면 검색날짜 UI / 일일 송순가능수 UI 변경되는거 Server 시간이 아니라 agent 시간기준으로 동작(XX:00:00에 동작)

        public bool bUseFileExceptionDescCheck { get; set; } = true;                           // 파일 예외신청 설명정보 필수 기업여부

        public bool bUsePartialFileAddInTransfer { get; set; } = false;         //'파일전송' 화면에서 등록시도한 파일목록에 정상파일과 오류파일이 함께 존재할 시 정상 파일에 대한 부분 등록 가능여부(true, false)

        public bool bUseChkHardSpace { get; set; } = false;                      //파일수신시 디바이스 용량 체크 여부

        public List<bool> bUseOver1Auth { get; set; } = null;                    // URL 자동전환 사용 유무 (망별로) ( true : 사용, false : 미사용 )

        public bool bUseLoginAfterTray { get; set; }                            //로그인 후 Tray 아이콘으로 이동

        public bool bUseFileApproveReason { get; set; }                        //파일 승인사유 입력 여부

        public bool bUseClipBoardApproveReason { get; set; }                   //클립보드 승인사유 입력여부

        public bool bUseNotinListForwardUrl { get; set; } = false;              //URL Redirect 정책에 안먹히는 URL forward 기능 사용유무

        public string strNotinListForwardUrl { get; set; } = "file:///C:/HANSSAK/OpenNetLink/wwwroot/Web/WebLinkInfoNotList.html";              // URL Redirect 정책에 안먹히는 URL forward할 주소

        public List<bool> bListUsePKIsendRecv { get; set; } = null;             // URL 자동전환 사용 유무 (망별로) ( true : 사용, false : 미사용 )
        
    }
}
