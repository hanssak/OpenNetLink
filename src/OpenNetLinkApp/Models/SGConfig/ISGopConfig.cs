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

        // notUse (MoveTo SGopConfig.json)
        // MoveTo. ISGopConfig.cs
        List<bool> bURLAutoTrans { get; }                           // URL 자동전환 사용 유무(groupID별로) ( true : 사용, false : 미사용 )
        List<bool> bURLAutoAfterMsg { get; }                        // URL 자동전환 후 사용자 알림 메시지 사용 여부(groupID별로) ( true : 사용, false : 미사용 )		
        List<string> strURLAutoAfterBrowser { get; }                // URL 자동전환 후 브라우저 창 처리방식 ( C : 닫기, N : 유지, F : 특정 URL 포워딩 )
        bool bRMouseFileAddAfterTrans { get; }                      // 마우스 우클릭 파일 추가 후 자동전송 사용 여부 ( true : 사용, false : 미사용 )
        bool bAfterBasicChk { get; }                                // 사후 결재 체크 기본 사용 유무 ( true : 체크, false : 체크 안함 )
        bool bRecvDownPathChange { get; }                           // 파일 수신 경로 변경 가능 여부 ( true : 가능, false : 불가능 )
        bool bManualRecvDownChange { get; }                         // 수동다운로드 사용 시 수신 폴더 변경 기능 ( true : 사용, false : 미사용)


        bool bFileRecvTrayFix { get; }                              // 파일 수신 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bApprTrayFix { get; }                                  // 결재자 승인대기 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bUserApprActionTrayFix { get; }                        // 사용자 승인완료 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bUserApprRejectTrayFix { get; }                        // 사용자 반려 알림 트레이 유지 여부 ( true : 유지, false : 유지 안함 )
        bool bExitTrayMove { get; }                                 // 종료 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        bool bStartTrayMove { get; }                                // 시작 시 트레이 이동 ( true : 트레이 이동, false : 종료 )
        bool bStartProgramReg { get; }                              // 시작 프로그램 등록 ( true : 시작프로그램 등록, false : 시작프로그램 등록 해제 )

        bool bScreenLock { get; }                                   // 화면잠금 사용 여부.(체크)
        bool bUseApprWaitNoti { get; }                              // 승인대기 알림 사용 여부.(체크)
        public bool bUseLogLevel { get; set; }                      // 로그 레벨 사용 여부
        public List<bool> listUseGpkiLogin { get; set; }            // GPKI 로그인 사용 여부



        public bool bScreenLockUserChange { get; set; }             //스크린락 사용자 임의 조작 가능 여부
        public bool bShowAdminInfo { get; set; }                    //대쉬보드에 관리자 정보 표시여부
        public bool bUseAppLoginType { get; set; }                  //사용자 지정 로그인타입 사용 여부
        public int LoginType { get; set; }                          //사용자 지정 로그인타입


        PAGE_TYPE enMainPageType { get; }                           // 메인페이지 (0 : NONE, 1 : DASHBOARD,  2 : TRANSFER)				
        public bool bUseNetOverAllsend { get; set; }                // 3망 전송에서 전체 사용자에게 보내는 기능 사용유무
        List<string> strForwardUrl { get; }                         // URL 자동전환 후 브라우저 창 Forward 할 주소 저장 ( F방식일대에만 사용 : 포워딩할 URL )
        public bool bFileDownloadBeforeReciving { get; set; }       // 파일포워드 사용시 PC 미수신한 상태에서도 다운로드 가능 유무
        public bool bUseFileCheckException { get; set; }            // 파일검사 예외신청 사용유무

        // Not Work - 설정값은 있지만 동작 구현 안되어 있음
        public bool bClipCopyAutoSend { get; }                                // 클립보드 복사 후 자동전송 기능 사용 유무 ( true : 사용, false : 미사용 )

        // Add
        public bool bNoApproveManageUI { get; }                                // 결재관리 No사용 유무 ( true : 결재관리UI / NoTi 없어짐, false : 기존설정대로사용 )

        public bool bEmptyfileTrans { get; }                                   // 0kb 파일 송수신 가능 유무

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // siteConfig Code Level에서 동작하던거 AppOPsetting.json 으로 옮겨 온거

        public bool bUserIDSave { get; set; }                     // 로그인한 ID 저장 여부
        public bool bAutoLogin { get; set; }                      // 자동로그인 사용 여부.
        public bool bAutoLoginCheck { get; set; }                 // 자동로그인 체크박스 체크여부.
        public bool bApprLineLocalSave { get; set; }              // 결재라인 로컬 저장 여부.
        public bool bTitleDescSameChk { get; set; }               // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
        public bool bApprLineChkBlock { get; set; }               // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
        public bool bDlpInfoDisplay { get; set; }                 // 전송/결재 관리 리스트에서 개인정보 검출 표시 유무 설정. ( true : 표시, false : 표시 안함 )
        public bool bApprDeptSearch { get; set; }                 // 결재자 검색 창의 타부서 수정 가능 여부.

        public int nApprStepLimit { get; set; }                   // 결재자 Step 제한 설정. ( 0 : 무제한, 그외 양수 제한 Step )
        public bool bDeputyApprTerminateDel { get; set; }         // 설정된 대결재자가 정보를 기한이 만료되면 삭제 할지 여부 ( true : 삭제, false : 삭제 안함)
        public bool bUserPWChange { get; set; }                   // 사용자 패스워드 변경 사용 여부.
        public string strPWChangeProhibitLimit { get; set; }      // 패스워드 사용금지 문자열 지정.
        public int nPWChangeApplyCnt { get; set; }                // 패스워드 변경 시 허용되는 자리수 지정.
        public bool bURLListPolicyRecv { get; set; }              // URL 리스트 정책 받기 사용 유무, 
        public string strInitPasswd { get; set; }                 // 초기 패스워드 정보.

        public bool bRecvFolderChange { get; set; }               // 수신 폴더 변경 사용 여부.
        public bool bUseUserRecvDownPath { get; set; }            // 로그인 유저별 다운로드 경로 사용 여부
        public bool bUseEmail { get; set; }                // 메일 결재 사용 유무.
        public bool bUsePCURL { get; set; }                       // PCURL 사용 유무.
        public bool bUsePublicBoard { get; set; }                 // 공지사항 사용 유무.
        public bool bUseCertSend { get; set; }                    // 공인인증서 전송 사용 유무.

        /// <summary>
        /// ////////////////////////////////
        /// </summary>

        public bool bUseClipCopyAndSend { get; set; }    // 
        public bool bRFileAutoSend { get; set; }    // 
        public bool bShowAfterApprAutoCheck { get; set; }    // 사후결재 기본 체크 사용유무 : 공통환경설정에 나오도록 할지 유무

        public bool bRecvFolderOpen { get; set; }    // 파일 수신후 폴더 열기 사용유무
        public bool bManualDownFolderChange { get; set; }    // 
        public bool bFileRecvAlarmRetain { get; set; }    // 
        public bool bApprCountAlarmRetain { get; set; }    // 승인대기 알림 유지 사용 유무
        public bool bApprCompleteAlarmRetain { get; set; }    // 승인완료 알림 유지 사용 유무
        public bool bApprRejectAlarmRetain { get; set; }    // 반려 알림 유지 사용 유무
        public bool bUseApprCountAlaram { get; set; }    // bUseApprCountAlaram
        public bool bUseCloseTrayMove { get; set; }    // 
        public bool bUseStartTrayMove { get; set; }    // 
        public bool bUseStartProgramReg { get; set; }    // 
        public bool bUseLanguageSet { get; set; }    // 

        public bool bUseMainPageType { get; set; }    // 

        public bool bViewFileFilter { get; set; }    // 
        public bool bViewSGSideBarUIBadge { get; set; }    // 
        public bool bViewSGHeaderUIAlarmNoriAllDel { get; set; }    // 
        public bool bUseForceUpdate { get; set; }    // 

        public bool bViewDlpApproverSelectMyDept { get; set; }    // 


        public bool bClipBoardNoApproveButFileTrans { get; set; }    // 
        public int nClipAfterApproveUseType { get; set; }    // 
        public bool bUseUserSelectFirstServer { get; set; }    // 

        public bool bUseFileForward { get; set; }    // 파일포워드 기능 사용유무

        public bool bUseClipAlarmType { get; set; }           // 

        /// <summary>
        /// ////////////////////////////////
        /// </summary>

        public bool bUseDenyPasswordZip { get; set; }          // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool bUseClipBoardFileTrans { get; set; }          // 파일형태로보내는 클립보드 사용 유무
        public bool bUseFileClipManageUI { get; set; }          // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool bUseFileClipApproveUI { get; set; }          // 파일형태로보내는 클립보드 결재UI 나오게할지 유무

        public bool bUseOneToMultiLogin { get; set; }          // 1번에 다중망 로그인 기능 사용유무
        public bool bUseOneByOneLogOut { get; set; }            // 1번에 다중망 로그인할때(bUseOneToMultiLogin = true) 에도 선택한 망만 로그인아웃하도록 할 건지유무

        public bool bUseFileExceptionDescCheck { get; }                        // 파일예외신청할때, 신청사유 정보 기입되어 있도록 할지 유무

        public bool bUseAgentTime1aClock { get; set; }          // 사후결재 정책, 자정에  검색화면 검색날짜 UI / 일일 송순가능수 UI 변경되는거 Server 시간이 아니라 agent 시간기준으로 동작(XX:00:00에 동작)

        public string strApproverSearchType { get; set; } //결재자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInApproverTree { get; set; } //결재자 관련 팝업 시 직접 입력하여 결재자를 검색할 수 있는 기능 사용 유무 (Input 컨트롤 표시 유무)

        public string strReceiverSearchType { get; set; } //수신자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInReceiverTree { get; set; } //수신자 관련 팝업 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public string strProxySearchType { get; set; } //대결재등록 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInProxyTree { get; set; } //대결재등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public string strSecurityApproverSearchType { get; set; } //보안결재자 등록 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInSecurityApproverTree { get; set; } //보안결재자 등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public string strApproveExtApproverSearchType { get; set; }     // 결재필수 확장자 검색됐을때, 결재자 검색방식
        public bool bUseApproveExt { get; set; }                        // 결재필수 확장자 결재하는 기능 사용유무

        public bool bUseInputSearchApproveExtTree { get; set; }         // 결재필수 확장자, 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public bool bUsePartialFileAddInTransfer { get; set; }  //'파일전송' 화면에서 등록시도한 파일목록에 정상파일과 오류파일이 함께 존재할 시 정상 파일에 대한 부분 등록 가능여부(true, false)

    }
}
