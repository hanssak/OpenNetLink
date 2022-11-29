using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace OpenNetLinkApp.Models.SGConfig
{
    public class SGSiteConfig : ISGSiteConfig
    {
        public bool m_bUserIDSave { get; set; } = false;                    // 로그인한 ID 저장 여부
        public bool m_bAutoLogin { get; set; } = false;                     // 자동로그인 사용 여부.
        public bool m_bAutoLoginCheck { get; set; } = false;                // 자동로그인 체크박스 체크여부.
        public bool m_bApprLineLocalSave { get; set; } = false;             // 결재라인 로컬 저장 여부.
        public int m_nZipPWBlock { get; set; } = 0;                         // zip 파일 패스워드 검사 여부 ( 0 : 사용 안함, 1 : 비번 걸려 있을 경우 차단,  2 : 비번이 안걸려 있을 경우 차단 )
        public bool m_bTitleDescSameChk { get; set; } = false;              // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
        public bool m_bApprLineChkBlock { get; set; } = false;              // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
        public bool m_bApprDeptSearch { get; set; } = true;                 // 결재자 검색 창의 타부서 수정 가능 여부.
        public bool m_bApprTreeSearch { get; set; } = false;                // 결재자 검색 부서트리 사용 유무.
        public bool m_bUserPWChange { get; set; } = false;                  // 사용자 패스워드 변경 사용 여부.
        public string m_strPWChangeProhibitLimit { get; set; } = "";        // 패스워드 사용금지 문자열 지정.
        public int m_nPWChangeApplyCnt { get; set; } = 9;                   // 패스워드 변경 시 허용되는 자리수 지정.
        public string m_strInitPasswd { get; set; } = "";                   // 초기 패스워드 정보.
        public bool m_bUseScreenLock { get; set; } = true;                  // 화면잠금 사용 여부
        public bool m_bUseClipBoard { get; set; } = true;                   // 클립보드 사용 여부
        public bool m_bUseURLRedirection { get; set; } = true;              // URL 리다이렉션 사용 여부
        public bool m_bUseFileSend { get; set; } = true;                    // 파일 전송 사용 여부
        public bool m_bRecvFolderChange { get; set; } = true;               // 수신 폴더 변경 사용 여부.
        public bool m_bUseUserRecvDownPath { get; set; } = true;           // 로그인 유저별 다운로드 경로 사용 여부
        public bool m_bUseEmail { get; set; } = false;               // 메일 관리/결재 사용 유무.
        public bool m_bUsePCURL { get; set; } = false;                      // PCURL 사용 유무.
        public bool m_bUseClipApprove { get; set; } = false;                // 클립보드 결재 사용 유무
        public bool m_bUsePublicBoard { get; set; } = false;                // 공지사항 사용 유무.
        public bool m_bUseCertSend { get; set; } = false;                   // 인증서 전송 사용 유무.
        //public bool m_bUseOSMaxFilePath { get; set; } = true;               // OS제공 최대 길이 사용 여부 (true : OS가 지원하는 최대한 길이 사용 false : filefullPath : 90, 파일/폴더이름길이 : 80) 

        public bool m_bUseDenyPasswordZip { get; set; } = false;         // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool m_bFileForward { get; set; } = false;               // 파일포워드기능 사용할지 유무
        public bool m_bUseClipBoardFileTrans { get; set; } = true;         // 파일형태로보내는 클립보드 사용 유무
        public bool m_bUseFileClipManageUI { get; set; } = true;         // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool m_bUseFileClipApproveUI { get; set; } = false;          // 파일형태로보내는 클립보드 결재UI 나오게할지 유무

        public bool m_bUseClipTypeSelectSend { get; set; } = true;         // 클립보드를 보낼때, 이미지 / Text를 사용자가 선택해서 보내는 기능 사용유무

        public bool m_bUseClipTypeTextFirstSend { get; set; } = false;         // 클립보드를 보낼때, Text 및 image Mixed 상태일때 Text를 우선적으로 보내도록 설정

        public bool m_bUseApproveAfterLimit { get; set; } = true;             // 파일전송시 사후결재 Count 제한 사용유무

        public bool m_bUseClipBoardApproveAfterLimit { get; set; } = true;       // 클립보드 파일전송시 사후결재 Count 제한 사용유무


        public bool m_bUseAppLoginType { get; set; } = false;                   // 사용자 지정 로그인타입 사용 여부

        public int m_nLoginType { get; set; } = 0;                              //사용자 지정 로그인타입 지정

        public bool m_bNoApproveManageUI { get; set; } = false;            // 결재관리 No사용 유무 ( true : 결재관리UI / NoTi 없어짐, false : 기존설정대로사용 )

        //public bool bUseAgentTime1aClock { get; set; } = false;         // 사후결재 정책, 자정에  검색화면 검색날짜 UI / 일일 송순가능수 UI 변경되는거 Server 시간이 아니라 agent 시간기준으로 동작(XX:00:00에 동작)

        public string strApproverSearchType { get; set; } = "SEARCH"; //결재자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInApproverTree { get; set; } = true; //결재자 관련 팝업 시 직접 입력하여 결재자를 검색할 수 있는 기능 사용 유무 (Input 컨트롤 표시 유무)

        public string strReceiverSearchType { get; set; } = "SEARCH"; //수신자 추가 시 부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInReceiverTree { get; set; } = true; //수신자 관련 팝업 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public string strProxySearchType { get; set; } = "SEARCH";     //대결재자 등록 시  부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInProxyTree { get; set; } = true; //대결재등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public string strSecurityApproverSearchType { get; set; } = "SEARCH";     //보안결재자 등록 시  부서 표시 방식을 SEARCH/TREE 타입 중 설정 (TREE 옵션일 경우, bApprDeptSearch 옵션 무효화)
        public bool bUseInputSearchInSecurityApproverTree { get; set; } = true; //보안결재자 등록 시 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)

        public string strApproveExtApproverSearchType { get; set; } = "SEARCH";     // 결재필수 확장자 검색됐을때, 결재자 검색방식
        public bool bUseApproveExt { get; set; } = true;                            // 결재필수 확장자 결재하는 기능 사용유무

        public bool bUseInputSearchApproveExtTree { get; set; } = false;         // 결재필수 확장자, 직접 입력하여 결재자를 검색알 수 있는 기능 사용 (Input 컨트롤 표시 유무)


        public bool m_bAccessAllDrive { get; set; } = false;                    // 모든 Drive에 접근하도록 할지여부

    }
}
