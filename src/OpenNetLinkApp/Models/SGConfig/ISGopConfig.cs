using System;
using System.Collections.Generic;
using System.Text;

using Serilog;
using Serilog.Events;
using AgLogManager;

namespace OpenNetLinkApp.Models.SGConfig

{
    public interface ISGopConfig
    {

        //로그인 관련 기능
        public bool bUseAppLoginType { get; set; }                       //사용자 지정 로그인타입 사용 여부
        public int LoginType { get; set; }                                   //사용자 지정 로그인타입 지정
        public bool bUseGpkiLogin { get; set; }                            // GPKI 로그인 사용 여부
        public bool bUserIDSave { get; set; }                                      // 로그인한 ID 저장 여부
        public bool bAutoLogin { get; set; }                                       // 자동로그인 사용 여부.
        public bool bAutoLoginCheck { get; set; }                                  // 자동로그인 체크박스 체크여부.
        public bool bUseUserSelectFirstServer { get; set; }   // 사용자가 처음접속하는 Server(Network) 를 선택할 수 있을지 유무
        public bool bUseOneToMultiLogin { get; set; }         // 1번에 다중망 로그인 기능 사용유무
        public bool bUseOneByOneLogOut { get; set; }         // 1번에 다중망 로그인 기능 사용때에도 로그아웃은 선택한 망에서 개별 로그아웃적영
        public bool bUseOver1Auth { get; set; }                        // 1단계 이상 인증 사용

        //패스워드
        public bool bUserPWChange { get; set; }                   // 사용자 패스워드 변경 사용 여부.
        public string strPWChangeProhibitLimit { get; set; }        // 패스워드 사용금지 문자열 지정.
        public int nPWChangeApplyCnt { get; set; }                   // 패스워드 변경 시 허용되는 자리수 지정.
        public string strInitPasswd { get; set; }    // 초기 패스워드 정보.(hsck@2301)
        public bool bUseIDAsInitPassword { get; set; } //ID와 동일한 비밀번호를 초기 PW로 사용할지 여부 (true : ID=PW로 로그인된 경우, 비밀번호 변경 요청)

        //파일 전송
        public bool bRFileAutoSend { get; set; }                          // 마우스 우클릭 후 자동전송 사용 유무( 환경설정 체크박스 보이고 안보이고)
        //public bool bRMouseFileAddAfterTrans { get; set; }               // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        public bool bUseNetOverAllsend { get; set; }                   //3망 전송에서 전체 사용자에게 보내는 기능 사용유무
        public bool bFileDownloadBeforeReciving { get; set; }             //파일포워드 사용시 PC 미수신한 상태에서도 다운로드 가능 유무
        public bool bNoApproveManageUI { get; set; }                           // 결재관리 No사용 유무 ( true : 결재관리UI / NoTi 없어짐, false : 기존설정대로사용 )
        public bool bEmptyfileTrans { get; set; }                                  // 0kb 파일 송수신 가능 유무
        public bool bTitleDescSameChk { get; set; }                              // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부
        public bool bApprLineChkBlock { get; set; }                     // 고정 결재라인 사용 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
        //public bool bRecvFolderOpen { get; set; }    // 파일 수신 후 폴더 열기 사용 유무
        public bool bRecvFolderChange { get; set; }             // 수신 폴더 변경 사용 여부.
        public bool bManualDownFolderChange { get; set; }   // 수동다운로드로 다운 시 폴더 선택 사용 유무
        //public bool bManualRecvDownChange { get; set; }         // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        public bool bUseUserRecvDownPath { get; set; }          // 로그인 유저별 다운로드 경로 사용 여부
        public bool bUseDenyPasswordZip { get; set; }       // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool bUseFileForward { get; set; }         // 파일포워드 기능 사용유무(환경설정)
        public bool bFileForward { get; set; }         // 파일포워드기능 사용할지 유무
        public bool bUsePartialFileAddInTransfer { get; set; }        //'파일전송' 화면에서 등록시도한 파일목록에 정상파일과 오류파일이 함께 존재할 시 정상 파일에 대한 부분 등록 가능여부(true, false)
        public bool bUseChkHardSpace { get; set; }                      //파일수신시 디바이스 용량 체크 여부
        public bool bUseFileApproveReason { get; set; }                     //파일 승인사유 입력 여부
        public bool bUseClipBoardApproveReason { get; set; }             //클립보드 승인사유 입력여부
        public bool bUseFileSelectDelete { get; set; }                    // 파일 선택 삭제 사용 유무
        public bool bUseCrossPlatformOSforFileName { get; set; }         // 윈도우에서 파일이름에 사용못하는 문자 막는지 유무
        public bool bUseTitleDescMinLength { get; set; }             //제목,설명 최소길이 제한 사용유무
        public bool bUseAgentBlockValueChange { get; set; }                   // tbl_agent_block 에 들어가는 Type 값을 WebManager에서 data를 보여줄 수 있는 형태로 변경(WebManager/NetLink와 맞춤)
        public bool bUseOSMaxFilePath { get; set; }                             // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 
        public bool bUseFileForwardDownNotRecv { get; set; }                     // 파일 수신되기전에 파일포워드로 다운로드 가능유무

        //클립보드
        public bool bUseClipBoard { get; set; }                   // 클립보드 사용 여부
        public bool bUseClipCopyAndSend { get; set; }    // 클립보드 복사 후 전송 사용 유무 ( 환경설정 체크박스 보이고 안보이고)
        //public bool bClipCopyAutoSend { get; set; }               // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        public bool bUseClipApprove { get; set; }              // 클립보드 결재 사용 유무
        public bool bClipBoardNoApproveButFileTrans { get; set; }   // 클립보드 파일전송 사용 형태로 결재없이 동작
        public int nClipAfterApproveUseType { get; set; }  // 클립보드 파일전송형태 전송때, 0:CheckBox 및 결재 설정대로, 1:사전, 2:사후 로 전송되게 적용
        public bool bUseClipBoardFileTrans { get; set; }        // 파일형태로보내는 클립보드 사용 유무
        public bool bUseFileClipManageUI { get; set; }       // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool bUseFileClipApproveUI { get; set; }        // 파일형태로보내는 클립보드 결재UI 나오게할지 유무
        public bool bUseClipTypeSelectSend { get; set; }       // 클립보드를 보낼때, 이미지 / Text를 사용자가 선택해서 보내는 기능 사용유무
        public bool bUseClipTypeTextFirstSend { get; set; }       // 클립보드를 보낼때, Text 및 image Mixed 상태일때 Text를 우선적으로 보내도록 설정

        //메일
        public bool bUseEmail { get; set; }               // 메일 관리/결재 사용 유무.
        public bool bUiDlpShow { get; set; } // 메일 관리/결재 에서 개인정보 검색항목 View 및 Search 기능 display 유무

        //URL Redirection (URL 반대망 전송)
        //public bool bURLAutoTrans { get; set; }                            // URL 자동전환 사용 유무 (망별로) ( true : 사용, false : 미사용 )
        public bool bUseURLRedirectionAlarm { get; set; }                                 // URL 자동전환 알림 사용 유무(환경설정)
        //public bool bURLAutoAfterMsg { get; set; }                            // URL 자동전환 후 사용자 알림 메시지 사용 여부(망별로) ( true : 사용, false : 미사용 )
        public bool bUseURLRedirectionAlarmType { get; set; }                             // URL 자동전환 알림 타입 선택 사용 유무(환경설정)
        //public string strURLAutoAfterBrowser { get; set; }                 // URL 자동전환 후 브라우저 창 처리방식(망별로) ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        //public string strForwardUrl { get; set; }                           // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )

        //결재 기능
        public bool bShowAfterApprAutoCheck { get; set; }                          // 사후결재 기본 체크 사용유무 : 공통환경설정에 나오도록 할지 유무
        //public bool bAfterBasicChk { get; set; }                                  // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        public bool bApprLineLocalSave { get; set; }                            // 결재라인 로컬 저장 여부.
        public bool bApprDeptSearch { get; set; }                              // 결재자 검색 창의 타부서 선택 가능 여부.
        public bool bViewDlpApproverSelectMyDept { get; set; }                   // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)
        public bool bUseAgentTime1aClock { get; set; }        // 사후결재 정책, 자정에  검색화면 검색날짜 UI / 일일 송순가능수 UI 변경되는거 Server 시간이 아니라 agent 시간기준으로 동작(XX:00:00에 동작)

        //알림 기능
        public bool bFileRecvAlarmRetain { get; set; }                          // 파일 수신 후 알림 유지 사용 유무(환경설정)
        //public bool bFileRecvTrayFix { get; set; }                                // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 ) -- 기능 안됨
        public bool bApprCountAlarmRetain { get; set; }                        // 승인대기 알림 유지 사용 유무(환경설정)
        //public bool bApprTrayFix { get; set; }                               // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprCompleteAlarmRetain { get; set; }                       // 승인완료 알림 유지 사용 유무(환경설정)
        //public bool bUserApprActionTrayFix { get; set; }                          // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprRejectAlarmRetain { get; set; }                        // 반려 알림 유지 사용 유무(환경설정)
        //public bool bUserApprRejectTrayFix { get; set; }                      // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUseApprCountAlaram { get; set; }                          // 승인대기 알림 사용 유무.(환경설정)
        //public bool bUseApprWaitNoti { get; set; }                                 // 승인대기 알림 사용 여부.(체크)
        public bool bUseClipAlarmType { get; set; }                               // clipboard 송수신 알림 형태 수정가능유무
        public bool bUseInitAlarmPerDay { get; set; }                           // 일일 이전 날짜 알림 자동 삭제
        public bool bUseInitMessagePerDay { get; set; }                     // 일일 이전 날짜 메세지 자동 삭제

        //부가 기능 (Tray, 자동 실행 등)
        public bool bUseMainPageType { get; set; }                                 // 메인화면 사용 여부(환경설정)
        //public PAGE_TYPE enMainPageType { get; set; }                               // 메인페이지(0 : DASHBOARD, 1 : TRANSFER)
        public bool bUseCloseTrayMove { get; set; }                              // 종료 시 트레이 사용 유무.(환경설정)
        //public bool bExitTrayMove { get; set; }                                 // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bUseStartTrayMove { get; set; }                              // 프로그램 시작 시 트레이 이동 사용 유무.
        public bool bUseLoginAfterTray { get; set; }                                        //로그인 후 Tray 아이콘으로 이동
        //public bool bStartTrayMove { get; set; }                               // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bUseStartProgramReg { get; set; }                             // 시작 프로그램 등록 사용 유무.(환경설정)
        //public bool bStartProgramReg { get; set; }                                // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )
        //public bool bScreenLockUserChange { get; set; }                          //스크린 잠금 사용자 임의 변경 가능여부(환경설정)
        //public bool bUseScreenLock { get; set; }                                  // 화면잠금 사용 여부.(체크)
        public bool bUseLogLevel { get; set; }                                    // 로그 레벨 사용 여부
        public bool bShowAdminInfo { get; set; }                                 //대시보드에 관리자 정보 표시 여부 
        public bool bUseFileCheckException { get; set; }                         // 파일검사 예외 신청( Virus / Apt )
        public bool bUsePCURL { get; set; }                  // PCURL 사용 유무.
        public bool bUsePublicBoard { get; set; }             // 공지사항 사용 유무.
        public bool bUseCertSend { get; set; }                 // 인증서 전송 사용 유무. (OTP 팝업 메뉴)
        public bool bUseApproveAfterLimit { get; set; }         // 파일전송시 사후결재 Count 제한 사용유무
        public bool bUseClipBoardApproveAfterLimit { get; set; }     // 클립보드 파일전송시 사후결재 Count 제한 사용유무
        public bool bUseAllProxyAuthority { get; set; }           //대결재자로서, 원결재자의 모든 권한을 위임받아 사용할지 유무
        public bool bUseWebLinkPreviewer { get; set; }             //결재미리보기(파일전송/클립보드) 시 WebLink 뷰어 사용 유무
        public string strWebLinkPreviewerURL { get; set; }     //WebLink 미리보기 사용 시 WebLink 주소 ( + AP001_Docs_Viewer.do 사용)
        public bool bUseLanguageSet { get; set; }    // 언어설정 사용 유무.
        public bool bViewFileFilter { get; set; }   // (환경설정) 확장자 제한 화면 표시 유무.
        public bool bViewSGSideBarUIBadge { get; set; }  // 왼쪽 메뉴들에서 Badge 나오게할지 유무 설정값
        public bool bViewSGHeaderUIAlarmNoriAllDel { get; set; }  // 상단 HeaderUI에서 Alarm, Noti 상에 Badge 전체 삭제 메뉴 나오게할지 유무
        public bool bUseForceUpdate { get; set; }   // 넘기는 기능 없이 무조건 업데이트 사용 유무
        public bool bUseForceBackgroundUpdate { get; set; } //무조건 업데이트 시 별도 팝업 및 클릭없이 자동 업데이트
        public bool bVisiblePolicyUpdateButton { get; set; } //정책 업데이트 버튼 보여주기
        public string strApproverSearchType { get; set; } //결재자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInApproverTree { get; set; }//결재자 관련 팝업 시 직접 입력하여 결재자를 검색할 수 있는 기능 사용 유무 (Input 컨트롤 표시 유무)
        public string strReceiverSearchType { get; set; } //수신자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInReceiverTree { get; set; } //수신자 관련 팝업 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public string strProxySearchType { get; set; }     //대결재자 등록 시  부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInProxyTree { get; set; } //대결재등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public string strSecurityApproverSearchType { get; set; }    //보안결재자 등록 시  부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInSecurityApproverTree { get; set; } //보안결재자 등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public string strApproveExtApproverSearchType { get; set; }   // 결재필수 확장자 검색됐을때, 결재자 검색방식
        public bool bUseInputSearchApproveExtTree { get; set; }      // 결재필수 확장자, 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public bool bUseApproveExt { get; set; }                           // 결재필수 확장자 결재하는 기능 사용유무
        public bool bUseFileExceptionDescCheck { get; set; }                     // 파일 예외신청 설명정보 필수 기입여부
        public bool bUsePKIsendRecv { get; set; }           // 인증서 전송 사용 유무 (망별로) ( true : 사용, false : 미사용 )

        public bool bPkiSendByFileTrans { get; set; }                 // 인증서 전송기능 파일전송 방법으로 전송할지, 클립보드 방식으로 전송할지 유무

    }
}
