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
        //공통 기능
        public bool PocMode { get; set; } = false;                                    //POC여부(첫 실행 시 IP 정보 입력 및 IP전환 기능 활성화)

        //로그인 관련 기능
        public bool bUseAppLoginType { get; set; } = false;                         //사용자 지정 로그인타입 사용 여부
        public int LoginType { get; set; } = 0;                                   //사용자 지정 로그인타입 지정
        public bool bUseGpkiLogin { get; set; } = false;                            // GPKI 로그인 사용 여부
        public bool bUserIDSave { get; set; } = false;                                      // 로그인한 ID 저장 여부
        public bool bAutoLogin { get; set; } = false;                                       // 자동로그인 사용 여부.
        public bool bAutoLoginCheck { get; set; } = false;                                  // 자동로그인 체크박스 체크여부.
        public bool bUseUserSelectFirstServer { get; set; } = false;   // 사용자가 처음접속하는 Server(Network) 를 선택할 수 있을지 유무
        public bool bUseOneToMultiLogin { get; set; } = false;         // 1번에 다중망 로그인 기능 사용유무
        public bool bUseOneByOneLogOut { get; set; } = false;         // 1번에 다중망 로그인 기능 사용때에도 로그아웃은 선택한 망에서 개별 로그아웃적영
        public bool bUseOver1Auth { get; set; } = false;                        // 1단계 이상 인증 사용
        public bool bVisibleLogOutButton { get; set; } = true;              //상단의 [로그아웃] 버튼 표시 여부

        /// <summary>
        /// -1 : 바로 차단, 0 : 접속유무를 사용자에게 문의, 1 : 강제 접속 진행
        /// </summary>
        public int nSessionDuplicate { get; set; } = -1;           // 로그인때, 세션중복시 동작값(-1 : 강제 접속 여부를 묻지 않고, 바로 차단

        //패스워드
        public bool bUserPWChange { get; set; } = false;                   // 사용자 패스워드 변경 사용 여부.
        public bool bUseGoogleOtp2FactorAuth { get; set; } = false;        // 구글 Otp를사용한 2차인증기능사용
        public string strPWChangeProhibitLimit { get; set; } = "";        // 패스워드 사용금지 문자열 지정.
        public int nPWChangeApplyCnt { get; set; } = 9;                   // 패스워드 변경 시 허용되는 자리수 지정.
        public int nPWkeyBoardLinearCharCnt { get; set; } = 3;            // 키보드 상에 연속된 키몇개가 맞으면 안되는지
        public int nPWsameCharCnt { get; set; } = 3;                      // 동일 문자 몇개까지 허용가능하지 확인
        public int nPWchangeRuleCnt { get; set; } = 4;                   // 패스워드 변경시 적용되는 Rule(대문자/소문자/숫자/특수 중에 몇가지 가 적용되어야 하는지)


        public string strInitPasswd { get; set; } = "1K27SdexltsW0ubSCJgsZw==";    // 초기 패스워드 정보.(hsck@2301)
        public bool bUseIDAsInitPassword { get; set; } = false; //ID와 동일한 비밀번호를 초기 PW로 사용할지 여부 (true : ID=PW로 로그인된 경우, 비밀번호 변경 요청)

        //파일 전송
        public bool bRFileAutoSend { get; set; } = false;                           // 마우스 우클릭 후 자동전송 사용 유무( 환경설정 체크박스 보이고 안보이고)
        //public bool bRMouseFileAddAfterTrans { get; set; } = false;                 // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        public bool bUseNetOverAllsend { get; set; } = false;                        //3망 전송에서 전체 사용자에게 보내는 기능 사용유무
        public bool bFileDownloadBeforeReciving { get; set; } = false;              //파일포워드 사용시 PC 미수신한 상태에서도 다운로드 가능 유무
        public bool bNoApproveManageUI { get; set; } = false;                             // 결재관리 No사용 유무 ( true : 결재관리UI / NoTi 없어짐, false : 기존설정대로사용 )

        public bool bUseApproveManageUIForce { get; set; } = false;                       // 결재UI 서버 결재 사용과 상관없이 무조건 보이게 설정
        public bool bEmptyfileTrans { get; set; } = false;                                      // 0kb 파일 송수신 가능 유무
        public bool bTitleDescSameChk { get; set; } = false;                                // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부
        public bool bApprLineChkBlock { get; set; } = true;                         // 고정 결재라인 사용 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
        //public bool bRecvFolderOpen { get; set; } = false;                  // 파일 수신 후 폴더 열기 사용 유무
        public bool bRecvFolderChange { get; set; } = false;               // 수신 폴더 변경 사용 여부.
        public bool bManualDownFolderChange { get; set; } = false;    // 수동다운로드로 다운 시 폴더 선택 사용 유무
        //public bool bManualRecvDownChange { get; set; } = false;          // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)
        public bool bUseUserRecvDownPath { get; set; } = false;           // 로그인 유저별 다운로드 경로 사용 여부
        public bool bUseDenyPasswordZip { get; set; } = true;         // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool bUseFileForward { get; set; } = false;          // 파일포워드 기능 사용유무
        public bool bFileForward { get; set; } = false;               // 전송관리 화면에서 파일 전송 컬럼 보여줄지 여부
        public bool bUsePartialFileAddInTransfer { get; set; } = false;         //'파일전송' 화면에서 등록시도한 파일목록에 정상파일과 오류파일이 함께 존재할 시 정상 파일에 대한 부분 등록 가능여부(true, false)
        public bool bUseChkHardSpace { get; set; } = true;                      //파일수신시 디바이스 용량 체크 여부
        public bool bUseFileApproveReason { get; set; } = false;                        //파일 승인사유 입력 여부
        public bool bUseClipBoardApproveReason { get; set; } = false;                //클립보드 승인사유 입력여부
        public bool bUseFileSelectDelete { get; set; } = false;                         // 파일 선택 삭제 사용 유무
        public bool bUseCrossPlatformOSforFileName { get; set; } = false;          // 윈도우에서 파일이름에 사용못하는 문자 막는지 유무
        public bool bUseTitleDescMinLength { get; set; } = false;             //제목,설명 최소길이 제한 사용유무
        public bool bUseAgentBlockValueChange { get; set; } = true;                       // tbl_agent_block 에 들어가는 Type 값을 WebManager에서 data를 보여줄 수 있는 형태로 변경(WebManager/NetLink와 맞춤)
        public bool bUseOSMaxFilePath { get; set; } = true;                               // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 
        public bool bUseFileForwardDownNotRecv { get; set; } = true;                         // 파일 수신되기전에 파일포워드로 다운로드 가능유무
        public string strInitTransferFileExplorerPathInWindow { get; set; } = "";//전송화면에서 초기 표시할 기본 경로 ("ROOT" / 명시된 경로 / "")       

        public bool bUiDlpShow { get; set; } = true;    // 메일 관리/결재 에서 개인정보 검색항목 View 및 Search 기능 display 유무
        public bool bUiFileExpiredDateShow { get; set; } = true;  // 전송관리 화면에서 파일 만료일 표시 여부
        public bool bShowInnerFileErrDetail { get; set; } = false; //파일 추가에 제외된 파일 리스트에 내부에 걸린 파일 리스트 전부 보여주기
        public bool bAllowDRM { get; set; } = true;                 //DRM 파일 전송 허용 여부 (true : 전송, false : 차단) - Default : true

        //클립보드

        public bool bDlpFoundSendContinue { get; set; } = false;  //  개인정보 검출 됐을때, 정보보호 결재자 없이 현재결재자에게 결재받고 송신되도록 할지 유무

        public bool bUseClipBoard { get; set; } = true;                  // 클립보드 사용 여부
        public bool bUseClipCopyAndSend { get; set; } = false;    // 클립보드 복사 후 전송 사용 유무 ( 환경설정 체크박스 보이고 안보이고)
        //public bool bClipCopyAutoSend { get; set; } = false;                // 클립보드 복사 후 전송 기능 사용 유무 ( true : 사용, false : 미사용 )
        public bool bUseClipApprove { get; set; } = false;                // 클립보드 결재 사용 유무
        public bool bClipBoardNoApproveButFileTrans { get; set; } = false;   // 클립보드 파일전송 사용 형태로 결재없이 동작
        public int nClipAfterApproveUseType { get; set; } = 0;   // 클립보드 파일전송형태 전송때, 0:CheckBox 및 결재 설정대로, 1:사전, 2:사후 로 전송되게 적용
        public bool bUseClipBoardFileTrans { get; set; } = false;         // 파일형태로보내는 클립보드 사용 유무
        public bool bUseFileClipManageUI { get; set; } = false;         // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool bUseFileClipApproveUI { get; set; } = false;          // 파일형태로보내는 클립보드 결재UI 나오게할지 유무
        public bool bUseClipTypeSelectSend { get; set; } = true;         // 클립보드를 보낼때, 이미지 / Text를 사용자가 선택해서 보내는 기능 사용유무
        public bool bUseClipTypeTextFirstSend { get; set; } = false;         // 클립보드를 보낼때, Text 및 image Mixed 상태일때 Text를 우선적으로 보내도록 설정

        //메일
        public bool bUseEmail { get; set; } = false;               // 메일 관리/결재 사용 유무.

        public bool bUseEmailOnly { get; set; } = false;               // 메일 관리/결재 만 사용하는 UI 사용 유무.
        

        //URL Redirection (URL 반대망 전송)
        //public bool bURLAutoTrans { get; set; } = false;                               // URL 자동전환 사용 유무 (망별로) ( true : 사용, false : 미사용 )
        public bool bUseURLRedirectionAlarm { get; set; } = false;                               // URL 자동전환 알림 사용 유무(환경설정)
        //public bool bURLAutoAfterMsg { get; set; } = false;                            // URL 자동전환 후 사용자 알림 메시지 사용 여부(망별로) ( true : 사용, false : 미사용 )
        public bool bUseURLRedirectionAlarmType { get; set; } = false;                             // URL 자동전환 알림 타입 선택 사용 유무(환경설정)
        //public string strURLAutoAfterBrowser { get; set; } = "F";                    // URL 자동전환 후 브라우저 창 처리방식(망별로) ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        //public string strForwardUrl { get; set; } = "file:\\\\C:\\HANSSAK\\OpenNetLink\\wwwroot\\Web\\WebLinkInfo.html";                             // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )

        //결재 기능
        public bool bShowAfterApprAutoCheck { get; set; } = false;                          // 사후결재 기본 체크 사용유무 : 공통환경설정에 나오도록 할지 유무
        //public bool bAfterBasicChk { get; set; } = false;                                   // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        public bool bApprLineLocalSave { get; set; } = true;                               // 결재라인 로컬 저장 여부.
        public bool bApprDeptSearch { get; set; } = true;                                   // 결재자 검색 창의 타부서 선택 가능 여부.
        public bool bViewDlpApproverSelectMyDept { get; set; } = false;                   // 정보보안 결재자 선택 화면 뜰때, 자기부서에 있는 사람들만 검색되어 나오도록 할 것이니 유무(true:자기부서만,false:전체)
        public bool bUseAgentTime1aClock { get; set; } = false;         // 사후결재 정책, 자정에  검색화면 검색날짜 UI / 일일 송순가능수 UI 변경되는거 Server 시간이 아니라 agent 시간기준으로 동작(XX:00:00에 동작)

        //알림 기능
        public bool bFileRecvAlarmRetain { get; set; } = false;                             // 파일 수신 후 알림 유지 사용 유무(환경설정)
        //public bool bFileRecvTrayFix { get; set; } = false;                                 // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 ) -- 기능 안됨
        public bool bApprCountAlarmRetain { get; set; } = false;                            // 승인대기 알림 유지 사용 유무(환경설정)
        //public bool bApprTrayFix { get; set; } = false;                                     // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprCompleteAlarmRetain { get; set; } = false;                         // 승인완료 알림 유지 사용 유무(환경설정)
        //public bool bUserApprActionTrayFix { get; set; } = false;                           // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bApprRejectAlarmRetain { get; set; } = false;                           // 반려 알림 유지 사용 유무(환경설정)
        //public bool bUserApprRejectTrayFix { get; set; } = false;                           // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        public bool bUseApprCountAlaram { get; set; } = true;                              // 승인대기 알림 사용 유무.(환경설정)
        //public bool bUseApprWaitNoti { get; set; } = true;                                  // 승인대기 알림 사용 여부.(체크)
        public bool bUseClipAlarmType { get; set; } = true;                                 // clipboard 송수신 알림 형태 수정가능유무
        public bool bUseInitAlarmPerDay { get; set; } = false;                              // 일일 이전 날짜 알림 자동 삭제
        public bool bUseInitMessagePerDay { get; set; } = false;                            // 일일 이전 날짜 메세지 자동 삭제


        //부가 기능 (Tray, 자동 실행 등)
        public bool bUseMainPageType { get; set; } = true;                                  // 메인화면 사용 여부
        //public PAGE_TYPE enMainPageType { get; set; } = PAGE_TYPE.NONE;                     // 메인페이지 리스트 (0 : DASHBOARD, 1 : TRANSFER)
        public bool bUseCloseTrayMove { get; set; } = false;                                // 종료 시 트레이 사용 유무.(환경설정)
        //public bool bExitTrayMove { get; set; } = false;                                    // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bUseStartTrayMove { get; set; } = false;                                // 프로그램 시작 시 트레이 이동 사용 유무.(환경설정)
        public bool bUseLoginAfterTray { get; set; } = false;                               //로그인 후 Tray 아이콘으로 이동
        //public bool bStartTrayMove { get; set; } = false;                                   // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        public bool bUseStartProgramReg { get; set; } = false;                              // 시작 프로그램 등록 사용 유무.(환경설정)
        //public bool bStartProgramReg { get; set; } = false;                                 // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )
        //public bool bScreenLockUserChange { get; set; } = false;                            //스크린 잠금 사용자 임의 변경 가능여부(환경설정)
        //public bool bUseScreenLock { get; set; } = false;                                   // 화면잠금 사용 여부.(체크)
        public bool bUseLogLevel { get; set; } = false;                                     // 로그 레벨 사용 여부
        public bool bShowAdminInfo { get; set; } = false;                                   //대시보드에 관리자 정보 표시 여부 
        public bool bUseFileCheckException { get; set; } = true;                           // 파일검사 예외 신청( Virus / Apt )
        public bool bUsePCURL { get; set; } = false;                      // PCURL 사용 유무.
        public bool bUsePublicBoard { get; set; } = false;                // 공지사항 사용 유무.
        public bool bUseCertSend { get; set; } = false;                   // 인증서 전송 사용 유무. (OTP 팝업 메뉴)
        public bool bUseApproveAfterLimit { get; set; } = false;             // 파일전송시 사후결재 Count 제한 사용유무
        public bool bUseClipBoardApproveAfterLimit { get; set; } = false;       // 클립보드 파일전송시 사후결재 Count 제한 사용유무
        public bool bUseAllProxyAuthority { get; set; } = false;             //대결재자로서, 원결재자의 모든 권한을 위임받아 사용할지 유무
        public bool bUseWebLinkPreviewer { get; set; } = false;               //결재미리보기(파일전송/클립보드) 시 WebLink 뷰어 사용 유무
        public string strWebLinkPreviewerURL { get; set; } = "218.145.246.25";     //WebLink 미리보기 사용 시 WebLink 주소 ( + AP001_Docs_Viewer.do 사용)
        public bool bUseLanguageSet { get; set; } = false;    // 언어설정 사용 유무.
        public bool bViewFileFilter { get; set; } = true;   // (환경설정) 확장자 제한 화면 표시 유무.
        public bool bViewSGSideBarUIBadge { get; set; } = false;   // 왼쪽 메뉴들에서 Badge 나오게할지 유무 설정값
        public bool bViewSGHeaderUIAlarmNoriAllDel { get; set; } = true;   // 상단 HeaderUI에서 Alarm, Noti 상에 Badge 전체 삭제 메뉴 나오게할지 유무
        public bool bUseForceUpdate { get; set; } = true;   // 넘기는 기능 없이 무조건 업데이트 사용 유무
        public bool bUseForceBackgroundUpdate { get; set; } = false; //무조건 업데이트 시 별도 팝업 및 클릭없이 자동 업데이트
        public bool bVisiblePolicyUpdateButton { get; set; } = true; //정책 업데이트 버튼 보여주기
        public string strApproverSearchType { get; set; } = "SEARCH"; //결재자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInApproverTree { get; set; } = true; //결재자 관련 팝업 시 직접 입력하여 결재자를 검색할 수 있는 기능 사용 유무 (Input 컨트롤 표시 유무)
        public string strReceiverSearchType { get; set; } = "SEARCH"; //수신자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInReceiverTree { get; set; } = true; //수신자 관련 팝업 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public string strProxySearchType { get; set; } = "SEARCH";     //대결재자 등록 시  부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInProxyTree { get; set; } = true; //대결재등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public string strSecurityApproverSearchType { get; set; } = "SEARCH";     //보안결재자 등록 시  부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInSecurityApproverTree { get; set; } = true; //보안결재자 등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public string strApproveExtApproverSearchType { get; set; } = "SEARCH";     // 결재필수 확장자 검색됐을때, 결재자 검색방식
        public bool bUseInputSearchApproveExtTree { get; set; } = false;         // 결재필수 확장자, 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)
        public bool bUseApproveExt { get; set; } = false;                            // 결재필수 확장자 결재하는 기능 사용유무
        public bool bUseFileExceptionDescCheck { get; set; } = false;                           // 파일 예외신청 설명정보 필수 기입여부
        public bool bUsePKIsendRecv { get; set; } = false;             // 인증서 전송 사용 유무 (망별로) ( true : 사용, false : 미사용 )
        public bool bUseToastInsteadOfOSNotification { get; set; } = false;                        //레지스트리 차단으로 OS 노티 사용 불가한 Site에서 OS노티 대신 Toast 사용 (Default/false) 
        public string strScrTimeoutLockType { get; set; } = "ScreenLock";                      //ScrLockTime의 시간초과로 타임아웃 발생 시, 처리타입(ScreenLock:화면잠금, LogOut:로그아웃, Exit: 프로그램 종료)

        public string strOKTAUrl { get; set; } = "";
        public bool bPkiSendByFileTrans { get; set; } = false;                // 인증서 전송기능 파일전송 방법으로 전송할지, 클립보드 방식으로 전송할지 유무
    }
}
