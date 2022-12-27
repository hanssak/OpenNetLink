using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGConfig

{
    public interface ISGSiteConfig
    {
        public bool m_bUserIDSave { get; set; }                     // 로그인한 ID 저장 여부
        public bool m_bAutoLogin { get; set; }                      // 자동로그인 사용 여부.
        public bool m_bAutoLoginCheck { get; set; }                 // 자동로그인 체크박스 체크여부.
        public bool m_bApprLineLocalSave { get; set; }              // 결재라인 로컬 저장 여부.
        public int m_nZipPWBlock { get; set; }                      // zip 파일 패스워드 검사 여부 ( 0 : 사용 안함, 1 : 비번 걸려 있을 경우 차단,  2 : 비번이 안걸려 있을 경우 차단 )
        public bool m_bTitleDescSameChk { get; set; }               // 파일 전송 시 제목과 설명의 연속된 동일 문자 체크 여부.
        public bool m_bApprLineChkBlock { get; set; }               // 고정 결재라인 차단 시 결재라인이 존재하지 않는 사용자에 대해 파일 전송 차단 여부 ( true : 전송 차단, false : 전송 허용 )
        public bool m_bApprDeptSearch { get; set; }                 // 결재자 검색 창의 타부서 수정 가능 여부.
        public bool m_bApprTreeSearch { get; set; }                 // 결재자 검색 부서트리 사용 유무.
        public bool m_bUserPWChange { get; set; }                   // 사용자 패스워드 변경 사용 여부.
        public string m_strPWChangeProhibitLimit { get; set; }      // 패스워드 사용금지 문자열 지정.
        public int m_nPWChangeApplyCnt { get; set; }                // 패스워드 변경 시 허용되는 자리수 지정.
        public string m_strInitPasswd { get; set; }                 // 초기 패스워드 정보.
        public bool m_bUseScreenLock { get; set; }                  // 화면잠금 사용 여부
        public bool m_bUseClipBoard { get; set; }                   // 클립보드 사용 여부
        public bool m_bUseURLRedirection { get; set; }              // URL 리다이렉션 사용 여부
        public bool m_bUseFileSend { get; set; }                    // 파일 전송 사용 여부
        //public bool m_bUseOSMaxFilePath { get; set; }               // OS제공 최대 길이 사용 여부
        public bool m_bRecvFolderChange { get; set; }               // 수신 폴더 변경 사용 여부.
        public bool m_bUseUserRecvDownPath { get; set; }            // 로그인 유저별 다운로드 경로 사용 여부
        public bool m_bUseEmail { get; set; }                // 메일 결재 사용 유무.
        public bool m_bUsePCURL { get; set; }                       // PCURL 사용 유무.
        public bool m_bUseClipApprove { get; set; }                 // 클립보드 결재 사용 유무
        public bool m_bUsePublicBoard { get; set; }                 // 공지사항 사용 유무.
        public bool m_bUseCertSend { get; set; }                    // 공인인증서 전송 사용 유무.



        public bool m_bUseDenyPasswordZip { get; set; }          // zip 같은 압축파일들 패스워드 걸려 있을때, 파일추가 안되게 할지 유무
        public bool m_bFileForward { get; set; }                // 파일포워드기능 사용할지 유무
        public bool m_bUseClipBoardFileTrans { get; set; }          // 파일형태로보내는 클립보드 사용 유무
        public bool m_bUseFileClipManageUI { get; set; }          // 파일형태로보내는 클립보드 관리UI 나오게할지 유무
        public bool m_bUseFileClipApproveUI { get; set; }          // 파일형태로보내는 클립보드 결재UI 나오게할지 유무

        public bool m_bUseClipTypeSelectSend { get; set; }          // 클립보드를 보낼때, 이미지 / Text를 사용자가 선택해서 보내는 기능 사용유무

        public bool m_bUseClipTypeTextFirstSend { get; set; }          // 클립보드를 보낼때, 이미지 / Text를 사용자가 선택해서 보내는 기능 사용유무

        public bool m_bUseApproveAfterLimit { get; set; }             // 파일전송시 사후결재 Count 제한 사용유무

        public bool m_bUseClipBoardApproveAfterLimit { get; set; }             // 클립보드 파일전송시 사후결재 Count 제한 사용유무

        public bool m_bUseAllProxyAuthority { get; set; }             //대결재자로서, 원결재자의 모든 권한을 위임받아 사용할지 유무

        public bool m_bUseWebLinkPreviewer { get; set; }                //결재미리보기(파일전송/클립보드) 시 WebLink 뷰어 사용 유무

        public string m_strWebLinkPreviewerURL { get; set; }           //WebLink 미리보기 사용 시 WebLink IP또는 도메인 주소 ( + AP001_Docs_Viewer.do 사용)


    }
}
